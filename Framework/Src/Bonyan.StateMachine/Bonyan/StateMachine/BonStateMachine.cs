namespace Bonyan.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class BonStateManager<TInstance> where TInstance : class, IBonStateModel
    {
        private readonly Dictionary<string, BonState<TInstance>> _stateCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, List<IWhenClause<TInstance>>> _eventHandlers = new();
        private readonly HashSet<Type> _definedEvents = new();
        public BonState<TInstance> Initial { get; }
        public BonState<TInstance> Final { get; }

        protected BonStateManager()
        {
            Initial = new BonState<TInstance>("Initial");
            Final = new BonState<TInstance>("Final");

            DefineState(Initial);
            DefineState(Final);

            Configure();
        }

        #region State and Event Definitions

        public void DefineState(BonState<TInstance> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (_stateCache.ContainsKey(state.Name))
                throw new InvalidOperationException($"State '{state.Name}' is already defined.");

            _stateCache[state.Name] = state;
        }

        public void DefineEvent<TEvent>() where TEvent : class
        {
            if (_definedEvents.Contains(typeof(TEvent)))
                throw new InvalidOperationException($"Event of type '{typeof(TEvent).Name}' is already defined.");

            _definedEvents.Add(typeof(TEvent));

            if (!_eventHandlers.ContainsKey(typeof(TEvent)))
            {
                _eventHandlers[typeof(TEvent)] = new List<IWhenClause<TInstance>>();
            }
        }

        public BonState<TInstance> GetState(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("State name cannot be null or empty.", nameof(name));

            if (_stateCache.TryGetValue(name, out var state))
                return state;

            throw new InvalidOperationException($"State '{name}' is not defined.");
        }

        #endregion

        #region When, Initially, and During

        protected WhenClause<TInstance, TEvent> When<TEvent>() where TEvent : class
        {
            if (!_eventHandlers.ContainsKey(typeof(TEvent)))
                throw new InvalidOperationException($"Event '{typeof(TEvent).Name}' is not defined.");

            var clause = new WhenClause<TInstance, TEvent>();

            _eventHandlers[typeof(TEvent)].Add(clause);
            return clause;
        }

        protected void Initially(params IWhenClause<TInstance>[] clauses)
        {
            During(Initial, clauses);
        }

        protected void During(BonState<TInstance> state, params IWhenClause<TInstance>[] clauses)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (clauses == null || clauses.Length == 0)
                throw new ArgumentException("At least one WhenClause must be specified.", nameof(clauses));

            foreach (var clause in clauses)
            {
                clause.SetTransitionFromState(state);
                if (!_eventHandlers.ContainsKey(clause.EventType))
                {
                    _eventHandlers[clause.EventType] = new List<IWhenClause<TInstance>>();
                }

                _eventHandlers[clause.EventType].Add(clause);
            }
        }

        protected void Finally(params Action<TInstance>[] actions)
        {
            if (actions == null || actions.Length == 0)
                throw new ArgumentException("At least one action must be specified.", nameof(actions));

            foreach (var action in actions)
            {
                Final.AddEntryAction(instance => Task.Run(() => action(instance)));
            }
        }

        #endregion

        #region Event Handling

        public async Task RaiseEvent<TEvent>(TInstance instance, TEvent eventData) where TEvent : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            if (!_eventHandlers.TryGetValue(typeof(TEvent), out var handlers))
                throw new InvalidOperationException($"No handlers defined for event '{typeof(TEvent).Name}'.");

            var currentState = GetState(instance.State);

            foreach (var handler in handlers.OfType<WhenClause<TInstance, TEvent>>())
            {
                if (handler.TransitionFromState?.Name == currentState.Name)
                {
                    await handler.ExecuteAsync(instance, eventData);
                    return;
                }
            }

            throw new InvalidOperationException($"No valid transitions for event '{typeof(TEvent).Name}' in state '{instance.State}'.");
        }

        #endregion

        protected abstract void Configure();
    }

    public class BonState<TInstance>
    {
        public string Name { get; }
        private readonly List<Func<TInstance, Task>> _entryActions = new();

        public BonState(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("State name cannot be null or empty.", nameof(name));

            Name = name;
        }

        public void AddEntryAction(Func<TInstance, Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _entryActions.Add(action);
        }

        public async Task ExecuteEntryActions(TInstance instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (var action in _entryActions)
            {
                await action(instance);
            }
        }
    }

    public interface IWhenClause<TInstance>
    {
        Type EventType { get; }
        BonState<TInstance>? TransitionFromState { get; }
        void SetTransitionFromState(BonState<TInstance> state);
    }

    public class WhenClause<TInstance, TEvent> : IWhenClause<TInstance> where TInstance : class, IBonStateModel where TEvent : class
    {
        public Type EventType => typeof(TEvent);
        public BonState<TInstance>? TransitionFromState { get; private set; }
        private Action<TInstance, TEvent>? _action;
        private BonState<TInstance>? _transitionToState;

        public WhenClause() {}

        public WhenClause<TInstance, TEvent> Then(Action<TInstance, TEvent> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public WhenClause<TInstance, TEvent> TransitionTo(BonState<TInstance> state)
        {
            _transitionToState = state ?? throw new ArgumentNullException(nameof(state));
            return this;
        }

        public void SetTransitionFromState(BonState<TInstance> state)
        {
            TransitionFromState = state ?? throw new ArgumentNullException(nameof(state));
        }

        public async Task ExecuteAsync(TInstance instance, TEvent eventData)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            _action?.Invoke(instance, eventData);

            if (_transitionToState != null)
            {
                instance.State = _transitionToState.Name;
                await _transitionToState.ExecuteEntryActions(instance);
            }
        }
    }

    public interface IBonStateModel
    {
        string State { get; set; }
    }
}
