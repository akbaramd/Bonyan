using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using BonyanTemplate.Domain.DomainEvents;

namespace BonyanTemplate.Domain.Entities;

public class Authors : BonAggregateRoot<AuthorId>
{
    public Authors()
    {
        AddDomainEvent(new BookCreated());
    }

    public string Title { get; set; } = string.Empty;

}

public class AuthorId : BonBusinessId<AuthorId>
{
}