namespace Bonyan.StateMachine;

public abstract class BonStateMachine<TStateInstance>
    where TStateInstance : class, IStateInstance
{
    private readonly Dictionary<string, BonState<TStateInstance>> _stateRegistry =
        new(StringComparer.OrdinalIgnoreCase);

    protected readonly Dictionary<Type, List<IEventHandler<TStateInstance>>> _eventHandlers = new();
    private readonly HashSet<Type> _definedEvents = new();

    // Error handling delegate
    public Action<TStateInstance, Exception>? OnError { get; set; }

    public BonState<TStateInstance> Initial { get; }
    public BonState<TStateInstance> Final { get; }

    public HashSet<Type> DefinedEvents => _definedEvents;

    protected BonStateMachine()
    {
        Initial = new BonState<TStateInstance>("Initial");
        Final = new BonState<TStateInstance>("Final");

        DefineState(Initial);
        DefineState(Final);
    }

    #region State and Event Definitions

    public void DefineState(BonState<TStateInstance> bonState)
    {
        if (bonState == null) throw new ArgumentNullException(nameof(bonState));
        if (_stateRegistry.ContainsKey(bonState.Name))
            throw new InvalidOperationException($"State '{bonState.Name}' is already defined.");

        _stateRegistry[bonState.Name] = bonState;
    }

    public void DefineEvent<TEvent>() where TEvent : class
    {
        if (_definedEvents.Contains(typeof(TEvent)))
            throw new InvalidOperationException($"Event of type '{typeof(TEvent).Name}' is already defined.");

        _definedEvents.Add(typeof(TEvent));

        if (!_eventHandlers.ContainsKey(typeof(TEvent)))
        {
            _eventHandlers[typeof(TEvent)] = new List<IEventHandler<TStateInstance>>();
        }
    }

    public BonState<TStateInstance> GetState(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Initial;

        if (_stateRegistry.TryGetValue(name, out var state))
            return state;

        throw new InvalidOperationException($"State '{name}' is not defined.");
    }

    #endregion

    #region State Transitions

    protected ContextAwareEventHandler<TStateInstance, TEvent> OnEvent<TEvent>() where TEvent : class
    {
        if (!_eventHandlers.ContainsKey(typeof(TEvent)))
            throw new InvalidOperationException($"Event '{typeof(TEvent).Name}' is not defined.");

        var handler = new ContextAwareEventHandler<TStateInstance, TEvent>(this);

        _eventHandlers[typeof(TEvent)].Add(handler);
        return handler;
    }

    protected void Initially(params IEventHandler<TStateInstance>[] handlers)
    {
        During(Initial, handlers);
    }

    protected void DuringAny(params IEventHandler<TStateInstance>[] handlers)
    {
        // Apply the handlers to all states.
        foreach (var state in _stateRegistry.Values)
        {
            During(state, handlers);
        }
    }

    protected void DuringStateTransition(BonState<TStateInstance> fromState, BonState<TStateInstance> toState, params IEventHandler<TStateInstance>[] handlers)
    {
        // Apply handlers specifically for state transitions from one state to another.
        foreach (var handler in handlers)
        {
            handler.SetOriginState(fromState);
            handler.TransitionToState = toState;
            _eventHandlers[handler.EventType].Add(handler);
        }
    }

    protected void During(BonState<TStateInstance> bonState, params IEventHandler<TStateInstance>[] handlers)
    {
        if (bonState == null) throw new ArgumentNullException(nameof(bonState));
        if (handlers == null || handlers.Length == 0)
            throw new ArgumentException("At least one event handler must be specified.", nameof(handlers));

        foreach (var handler in handlers)
        {
            handler.SetOriginState(bonState);
            if (!_eventHandlers.ContainsKey(handler.EventType))
            {
                _eventHandlers[handler.EventType] = new List<IEventHandler<TStateInstance>>();
            }

            _eventHandlers[handler.EventType].Add(handler);
        }
    }

    protected void Finally(params Action<TStateInstance>[] actions)
    {
        if (actions == null || actions.Length == 0)
            throw new ArgumentException("At least one action must be specified.", nameof(actions));

        foreach (var action in actions)
        {
            Final.AddEntryAction(instance => Task.Run(() => action(instance)));
        }
    }

    #endregion

    #region Event Processing

    public async Task ProcessAsync<TEvent>(TStateInstance instance, TEvent eventData) where TEvent : class
    {
        await ProcessAsync(instance, null, eventData);
    }

    public async Task ProcessAsync<TEvent>(TStateInstance instance, object? context, TEvent eventData)
        where TEvent : class
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (eventData == null) throw new ArgumentNullException(nameof(eventData));

        try
        {
            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handlers))
                throw new InvalidOperationException($"No handlers defined for event '{typeof(TEvent).Name}'.");

            var currentState = GetState(instance.State);

            // Execute event handlers with respect to the current state.
            foreach (var handler in handlers.OfType<IContextAwareEventHandler<TStateInstance, TEvent>>())
            {
                if (handler.OriginState?.Name == currentState.Name)
                {
                    await handler.ExecuteAsync(instance, context, eventData);
                    return;
                }
            }

            // Custom error handling, can be overridden in derived classes.
            HandleEventProcessingError(instance, eventData);
        }
        catch (Exception ex)
        {
            OnErrorHandled(instance, ex);
        }
    }

    // Virtual hook for customizing event processing error handling.
    protected virtual void HandleEventProcessingError<TEvent>(TStateInstance instance, TEvent eventData) where TEvent : class
    {
        // Default error handling or logging could go here.
        Console.WriteLine($"Error processing event {typeof(TEvent).Name} for instance {instance.State}: {eventData}");
    }

    protected virtual void OnErrorHandled(TStateInstance instance, Exception exception)
    {
        // Hook for error handling that can be customized in derived classes.
        Console.WriteLine($"Error in state machine: {exception.Message}");
    }

    #endregion

    #region State Management Helpers

    // Virtual method to allow derived classes to have control over state transition behavior.
    protected virtual Task ExecuteStateTransitionAsync(TStateInstance instance, BonState<TStateInstance> state)
    {
        return state.ExecuteEntryActionsAsync(instance);
    }

    #endregion
}

public class BonState<TStateInstance>
{
    public string Name { get; }
    private readonly List<Func<TStateInstance, Task>> _entryActions = new();
    private readonly List<Func<TStateInstance, Task>> _finalizeActions = new();

    public BonState(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("State name cannot be null or empty.", nameof(name));

        Name = name;
    }

    public void AddEntryAction(Func<TStateInstance, Task> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        _entryActions.Add(action);
    }

    public void AddFinalizeAction(Func<TStateInstance, Task> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        _finalizeActions.Add(action);
    }

    public async Task ExecuteEntryActionsAsync(TStateInstance instance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        foreach (var action in _entryActions)
        {
            await action(instance);
        }
    }

    public async Task ExecuteFinalizeActionsAsync(TStateInstance instance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        foreach (var action in _finalizeActions)
        {
            await action(instance);
        }
    }
}

public interface IEventHandler<TStateInstance>
{
    Type EventType { get; }
    BonState<TStateInstance>? OriginState { get; }
    BonState<TStateInstance>? TransitionToState { get; set; }
    void SetOriginState(BonState<TStateInstance> bonState);

    // Finalize actions inside event handler
    void AddFinalizeAction(Func<TStateInstance, Task> finalizeAction);
}

public interface IContextAwareEventHandler<TStateInstance, TEvent> : IEventHandler<TStateInstance>
{
    Task ExecuteAsync(TStateInstance instance, object? context, TEvent eventData);
    Task ExecuteAsync(TStateInstance instance, TEvent eventData);
}

public class ContextAwareEventHandler<TStateInstance, TEvent> : IContextAwareEventHandler<TStateInstance, TEvent>
    where TStateInstance : class, IStateInstance
    where TEvent : class
{
    private readonly BonStateMachine<TStateInstance> _stateMachine;
    public Type EventType => typeof(TEvent);
    public BonState<TStateInstance>? OriginState { get; set; }
    private Action<TStateInstance, object?, TEvent>? _action;
    public BonState<TStateInstance>? TransitionToState { get; set; }
    private readonly List<Func<TStateInstance, Task>> _finalizeActions = new();

    public ContextAwareEventHandler(BonStateMachine<TStateInstance> stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public ContextAwareEventHandler<TStateInstance, TEvent> Then(Action<TStateInstance, object?, TEvent> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        return this;
    }

    public ContextAwareEventHandler<TStateInstance, TEvent> Then(Action<TStateInstance, TEvent> action)
    {
        this.Then((instance, _, eventData) => action(instance, eventData));
        return this;
    }

    public ContextAwareEventHandler<TStateInstance, TEvent> TransitionTo(BonState<TStateInstance> bonState)
    {
        TransitionToState = bonState ?? throw new ArgumentNullException(nameof(bonState));
        return this;
    }
        
    public ContextAwareEventHandler<TStateInstance, TEvent> Finalize()
    {
        TransitionTo(_stateMachine.Final);
        return this;
    }

    public void SetOriginState(BonState<TStateInstance> bonState)
    {
        OriginState = bonState ?? throw new ArgumentNullException(nameof(bonState));
    }

    public void AddFinalizeAction(Func<TStateInstance, Task> finalizeAction)
    {
        if (finalizeAction == null) throw new ArgumentNullException(nameof(finalizeAction));
        _finalizeActions.Add(finalizeAction);
    }

    public async Task ExecuteAsync(TStateInstance instance, object? context, TEvent eventData)
    {
        if (_action == null) throw new InvalidOperationException("No action defined.");
        _action(instance, context, eventData);

        if (TransitionToState != null)
        {
            instance.State = TransitionToState.Name;
            await TransitionToState.ExecuteEntryActionsAsync(instance);
        }

        // Execute finalize actions for the handler.
        foreach (var finalizeAction in _finalizeActions)
        {
            await finalizeAction(instance);
        }
    }

    public Task ExecuteAsync(TStateInstance instance, TEvent eventData)
    {
        return ExecuteAsync(instance, null, eventData);
    }
}

public interface IStateInstance
{
    string State { get; set; }
}