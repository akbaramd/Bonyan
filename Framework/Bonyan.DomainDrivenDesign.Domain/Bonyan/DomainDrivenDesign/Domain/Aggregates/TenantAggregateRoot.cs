using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class TenantAggregateRoot : AggregateRoot, ITenant
{
  // This class inherits everything from Entity and implements IAggregateRoot
  // No additional members are required unless specific logic is needed for non-generic aggregate roots.
  public string Tenant { get; set; } = default!;
}

/// <summary>
/// Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class TenantAggregateRoot<TKey> : AggregateRoot<TKey>, ITenant
{
  // This class inherits everything from Entity<TKey> and implements IAggregateRoot
  // No additional members are required unless specific logic is needed for generic aggregate roots.
  public string Tenant { get; set; } = default!;
}
