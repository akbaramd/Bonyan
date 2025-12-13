using Bonyan.Mediators;

namespace Bonyan.Mediator.Tests
{
    public class SampleCommand : IBonCommand<string>
    {
        public string Payload { get; set; }
    }

    public class SampleQuery : IBonCommand<string>
    {
        public string Query { get; set; }
    }

    public class SampleEvent : IBonEvent
    {
        public string EventData { get; set; }
    }
}



namespace Bonyan.Mediator.Tests
{
    public class SampleCommandHandler : IBonCommandHandler<SampleCommand, string>
    {
        public Task<string> HandleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Handled: {command.Payload}");
        }
    }

    public class SampleQueryHandler : IBonCommandHandler<SampleQuery, string>
    {
        public Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Query Result: {query.Query}");
        }
    }

    public class SampleEventHandler : IBonEventHandler<SampleEvent>
    {
        public Task HandleAsync(SampleEvent eventMessage, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Event Handled: {eventMessage.EventData}");
            return Task.CompletedTask;
        }
    }
}
