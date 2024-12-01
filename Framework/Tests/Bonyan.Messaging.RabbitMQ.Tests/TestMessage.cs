using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class TestCommand
{
    public string Content { get; set; }
}

public class TestResponse
{
    public string Content { get; set; }
}
public class NotHandledResponse
{
    public string Content { get; set; }
}

public class TestEvent
{
    public string Content { get; set; }
}

public class TestEventConsumer : IBonMessageConsumer<TestEvent>
{
    public Task ConsumeAsync(BonMessageContext<TestEvent> context, CancellationToken cancellationToken)
    {
        return context.ReplyAsync(context.Message);
    }
}