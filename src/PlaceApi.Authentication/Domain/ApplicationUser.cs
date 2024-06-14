using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using PlaceApi.SharedKernel;

namespace PlaceApi.Authentication.Domain;

public class ApplicationUser : IdentityUser, IHaveDomainEvents
{
    public string FullName { get; set; } = string.Empty;

    private readonly List<DomainEventBase> _domainEvents = [];

    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) =>
        _domainEvents.Add(domainEvent);

    void IHaveDomainEvents.ClearDomainEvents() => _domainEvents.Clear();
}
