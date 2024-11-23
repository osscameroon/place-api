using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.EF;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    IExecutionStrategy CreateExecutionStrategy();
    Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default);
}
