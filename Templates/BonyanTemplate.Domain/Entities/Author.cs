using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.ValueObjects;
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