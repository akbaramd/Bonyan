using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using BonyanTemplate.Domain.DomainEvents;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Authors : AggregateRoot<AuthorId>,IMultiTenant
{
  public string Title { get; set; } = string.Empty;

  public Authors()
  {
    AddDomainEvent(new BookCreated());
  }

  public Guid? TenantId { get; }
}


public class AuthorId : BusinessId<AuthorId>{}