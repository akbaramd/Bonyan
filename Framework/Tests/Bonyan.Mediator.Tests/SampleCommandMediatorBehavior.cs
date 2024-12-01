using Bonyan.Mediators;

namespace Bonyan.Mediator.Tests
{
    public class SampleCommandMediatorBehavior<TRequest, TResponse> : IBonMediatorBehavior<TRequest, TResponse>
        where TRequest : SampleCommand
    {
        public async Task<TResponse> HandleAsync(
            TRequest request,
            Func<Task<TResponse>> next,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(TestStrings.BehaviorPreProcessingMessage + request.Payload);
            var result = await next();
            Console.WriteLine(TestStrings.BehaviorPostProcessingMessage + request.Payload);
            return result;
        }
    }

    public class SampleEventMediatorBehavior<TEvent> : IBonMediatorBehavior<TEvent>
        where TEvent : SampleEvent
    {
        public async Task HandleAsync(
            TEvent @event,
            Func<Task> next,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(TestStrings.BehaviorPreProcessingMessage + @event.EventData);
            await next();
            Console.WriteLine(TestStrings.BehaviorPostProcessingMessage + @event.EventData);
        }
    }
}