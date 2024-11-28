using Bonyan.StateMachine;
using System;

public class TestStateMachine : BonStateManager<TestStateMachine.TestInstance>
{
    // Define states
    public BonState<TestInstance> Registered { get; } = new(nameof(Registered));
    public BonState<TestInstance> Completed { get; } = new(nameof(Completed));
    public BonState<TestInstance> Confirmed { get; } = new(nameof(Confirmed));

    // Define events


    protected override void Configure()
    {
        // Register states
        DefineState(Registered);
        DefineState(Confirmed);
        DefineState(Completed);

        // Register events
        DefineEvent<RegisterEvent>();
        DefineEvent<ConfirmEvent>();
        DefineEvent<CompleteEvent>();

        // Define transitions and initial state behavior
        Initially(
            When<RegisterEvent>()
                .Then((instance, eventData) =>
                {
                    Console.WriteLine($"Registered: {eventData.Message}");
                    instance.Id = eventData.UserId;
                })
                .TransitionTo(Registered)
        );

        During(
            Registered,
            When<ConfirmEvent>()
                .Then((instance, eventData) =>
                {
                    Console.WriteLine($"Confirmed: {eventData.ApproverId}");
                })
                .TransitionTo(Confirmed),
            When<CompleteEvent>()
                .Then((instance, eventData) =>
                {
                    Console.WriteLine($"Completed: {eventData.Timestamp}");
                })
                .TransitionTo(Completed)
        );
    }

    public class TestInstance : IBonStateModel
    {
        public string Id { get; set; }
        public string State { get; set; } = "Initial";
    }

    // Event payload classes
    public class RegisterEvent
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }

    public class ConfirmEvent
    {
        public string ApproverId { get; set; }
    }

    public class CompleteEvent
    {
        public DateTime Timestamp { get; set; }
    }
}
