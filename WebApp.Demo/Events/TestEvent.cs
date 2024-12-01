using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;

namespace WebApp.Demo.Events;

public class TestEvent : IBonDomainEvent
{
    public string Context { get; set; }
}

public class TestEventResponse
{
    public string Context { get; set; }
}

public class BookCreatedEventConsumer : IBonMessageConsumer<TestEvent>
{
    public async Task ConsumeAsync(BonMessageContext<TestEvent> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("BookCreatedEventConsumer | " + context.Message.Context);
        await context.ReplyAsync(new TestEventResponse()
        {
            Context = context.Message.Context
        });
    }
}

public class BookCreatedEventHadnler : IBonEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("BookCreatedEventHadnler | " + @event.Context);
        return Task.CompletedTask;
    }
}