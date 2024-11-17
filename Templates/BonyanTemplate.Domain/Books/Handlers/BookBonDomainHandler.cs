using Bonyan.Messaging.Abstractions;
using BonyanTemplate.Domain.Books.DomainEvents;

namespace BonyanTemplate.Domain.Books.Handlers;

public class BookBonDomainHandler : IBonMessageConsumer<BookCreated>
{
    public Task ConsumeAsync(BookCreated domainEvent , CancellationToken? cancellationToken )
    {
        Console.WriteLine("BookBonDomainHandler");
        return Task.CompletedTask;
    }
}