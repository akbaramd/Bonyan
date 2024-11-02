using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Books : AggregateRoot<BookId>, IMultiTenant
{
    public string Title { get; set; } = string.Empty;
    public BookStatus Status { get; set; } = BookStatus.Available;
    public Guid? TenantId { get; set; }
}

public class BookId : BusinessId<BookId>
{
}