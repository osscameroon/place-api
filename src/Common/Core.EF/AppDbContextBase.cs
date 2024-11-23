using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain.Event;
using Common.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Exception = System.Exception;
using IsolationLevel = System.Data.IsolationLevel;

namespace Core.EF;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private IDbContextTransaction _currentTransaction;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected AppDbContextBase(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder) { }

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken
        );
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default)
    {
        IExecutionStrategy strategy = CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken
            );
            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        List<IAggregate> domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        ImmutableList<IDomainEvent> domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToImmutableList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        //ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (EntityEntry entry in ex.Entries)
            {
                PropertyValues? databaseValues = await entry.GetDatabaseValuesAsync(
                    cancellationToken
                );

                if (databaseValues == null)
                {
                    throw;
                }

                // Refresh the original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    private void OnBeforeSaving()
    {
        try
        {
            string? nameIdentifier = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            long.TryParse(nameIdentifier, out long userId);

            foreach (EntityEntry<IAggregate> entry in ChangeTracker.Entries<IAggregate>())
            {
                bool isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));

                if (isAuditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = DateTime.Now;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.Version++;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Version++;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("try for find IAggregate", ex);
        }
    }
}
