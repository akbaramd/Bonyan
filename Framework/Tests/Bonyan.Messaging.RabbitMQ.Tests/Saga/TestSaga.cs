using Bonyan.Messaging.Saga;
using Bonyan.StateMachine;

namespace Bonyan.Messaging.RabbitMQ.Tests.Saga;

public class TestSagaInstance : IStateInstance
{
    public string State { get; set; } = string.Empty;
}

// Sample Saga Definition
public class TestMessagingSaga : BonMessagingSagaMachine<TestSagaInstance>
{
    public TestMessagingSaga()
    {
        var inProgress = new BonState<TestSagaInstance>("InProgress");
        var completed = new BonState<TestSagaInstance>("Completed");

        DefineState(inProgress);
        DefineState(completed);

        DefineEvent<StartEvent>();
        DefineEvent<UnknownEvent>();
        DefineEvent<CompleteEvent>();
        DefineEvent<ProgressEvent>();

        Initially(
            OnEvent<StartEvent>()
                .Publish((c, v) => { return new CompleteEvent() { Content = v.Message.Content }; })
                .TransitionTo(inProgress)
        );

        During(inProgress,
            OnEvent<CompleteEvent>()
                .Send("complete-command.queue", (i, e) => new CompleteCommand { Content = "Complete Processing" })
                .TransitionTo(completed)
        );


        Finally(instance => instance.State = Final.Name);
    }
}

// Sample Events
public class StartEvent
{
    public string Content { get; set; }
}

public class CompleteEvent
{
    public string Content { get; set; }
}

public class UnknownEvent
{
    public string Content { get; set; }
}

public class ProgressEvent
{
    public string Content { get; set; }
}

// Sample Commands
public class CompleteCommand
{
    public string Content { get; set; }
}

public class CompletionResponse
{
    public string Content { get; set; }
}