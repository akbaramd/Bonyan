namespace Bonyan.Mediators;

public class BonMediatorMessagingBehavior<TMessage> : IBonMediatorBehavior<TMessage> where TMessage : class
{
    private readonly IBonMessageBus _messageBus;

    public BonMediatorMessagingBehavior(IBonMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(TMessage request, Func<Task> next, CancellationToken cancellationToken)
    {
        if (request is IBonEvent @event)
        {
            await _messageBus.PublishAsync(@event, cancellationToken: cancellationToken);
        }
  
        await next();
        
    }
}