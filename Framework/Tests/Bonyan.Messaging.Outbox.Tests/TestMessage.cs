using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging.Outbox.Tests;

public class TestCommand : MessageRequestBase<TestResponse>
{
    public string Content { get; set; } = string.Empty;
}

public class TestResponse
{
    public string Content { get; set; } = string.Empty;
}
public class NotHandledResponse
{
    public string Content { get; set; } = string.Empty;
}

public class TestEvent : MessageEventBase
{
    public string Content { get; set; } = string.Empty;
}

public class TestEventConsumer : IBonMessageConsumer<TestEvent>
{
    public Task ConsumeAsync(BonMessageContext<TestEvent> context, CancellationToken cancellationToken)
    {
        return context.ReplyAsync(context.Message);
    }
}