using Bonyan.StateMachine;
using Stateless;

namespace Bonyan.Messaging.Saga;

public abstract class BonMessagingSagaMachine<TInstance> : BonStateManager<TInstance> where TInstance : class, IBonStateModel
{
    private readonly IBonMessageBus _messageBus;

    protected BonMessagingSagaMachine(IBonMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    
    
    
}