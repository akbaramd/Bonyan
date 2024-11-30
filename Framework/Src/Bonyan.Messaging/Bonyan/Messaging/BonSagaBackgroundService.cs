using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Saga;
using Bonyan.StateMachine;
using static System.DateTime;

namespace Bonyan.Messaging
{
    public class BonSagaBackgroundService : BackgroundService
    {
        private readonly IBonMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly BonMessagingConfiguration _messagingConfiguration;
        private readonly IBonStateStore _stateStore;

        // In-memory storage for saga states

        public BonSagaBackgroundService(
            IBonMessageBus messageBus,
            IServiceProvider serviceProvider,
            BonMessagingConfiguration messagingConfiguration, IBonStateStore stateStore)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messagingConfiguration =
                messagingConfiguration ?? throw new ArgumentNullException(nameof(messagingConfiguration));
            _stateStore = stateStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var sagaRegistration in _messagingConfiguration.GetSagaRegistrations())
            {
                var sagaType = sagaRegistration.SagaType;
                var instanceType = sagaType.BaseType?.GetGenericArguments()[0];

                if (instanceType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine the generic type for saga {sagaType.Name}.");
                }

                // Register events without causing unintended disposal
                RegisterSagaEventsGeneric(sagaRegistration.QueueName,sagaType, instanceType, stoppingToken);
            }

            await Task.CompletedTask; // Keep the service alive
        }

        private void RegisterSagaEventsGeneric(string queueName,Type sagaType, Type instanceType, CancellationToken stoppingToken)
        {
            var registerMethod = GetType()
                .GetMethod(nameof(RegisterSagaEvents),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.MakeGenericMethod(sagaType, instanceType);

            if (registerMethod == null)
            {
                throw new InvalidOperationException($"Unable to find method {nameof(RegisterSagaEvents)}.");
            }

            registerMethod.Invoke(this, new object[] { queueName,stoppingToken });
        }

        private void RegisterSagaEvents<TSaga, TInstance>(string queueName,CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            // Create a new scope to ensure services aren't disposed when the background service stops
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the required saga instance in the correct scope
                var saga = scope.ServiceProvider.GetRequiredService<TSaga>();

                // Subscribe to events within the scope of the saga
                foreach (var eventType in saga.DefinedEvents)
                {
                    SubscribeToSagaEventGeneric<TSaga, TInstance>(queueName,saga, eventType, stoppingToken);
                }
            }
        }

        private void SubscribeToSagaEventGeneric<TSaga, TInstance>(string queueName,TSaga saga, Type eventType,
            CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            var subscribeMethod = GetType()
                .GetMethod(nameof(SubscribeToSagaEvent),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.MakeGenericMethod(typeof(TSaga), typeof(TInstance), eventType);

            if (subscribeMethod == null)
            {
                throw new InvalidOperationException($"Unable to find method {nameof(SubscribeToSagaEvent)}.");
            }

            subscribeMethod.Invoke(this, new object[] { queueName,saga, stoppingToken });
        }

        private async Task SubscribeToSagaEvent<TSaga, TInstance, TEvent>(string queueName,TSaga saga, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
            where TEvent : class
        {
            // Ensure the message bus is not disposed of and subscribe to the event
            _messageBus.Subscribe<TEvent>($"{queueName}.saga.{typeof(TEvent).Name}", async context =>
            {
                if (stoppingToken.IsCancellationRequested) return;


                try
                {
                    // Load the saga state
                    var state = await LoadSagaStateAsync<TSaga, TInstance>(saga, context.CorrelationId, stoppingToken);

                    // Raise the event in the saga's state machine
                    await saga.ProcessAsync(state, context,context.Message);

                    // Save the updated saga state
                    await SaveSagaStateAsync(saga, state, context.CorrelationId, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Add logging or error handling here
                    Console.WriteLine($"Error processing event {typeof(TEvent).Name}: {ex.Message}");
                }
            });
        }

        private async Task<TInstance> LoadSagaStateAsync<TSaga, TInstance>(TSaga saga, string correlationId,
            CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            // Attempt to retrieve the state from in-memory storage
            var stateData = await _stateStore.LoadStateAsync(correlationId);
            // If not found, create a new instance
            if (stateData == null)
            {
                return new TInstance { }; // Return a new instance with the initial state
            }

            var data = JsonSerializer.Deserialize<TInstance>(stateData.StateData);
            if (data == null)
            {
                return new TInstance { }; // Return a new instance with the initial state
            }

            return data;
        }

        private async Task SaveSagaStateAsync<TSaga, TInstance>(TSaga saga, TInstance state, string correlationId,
            CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance
        {
            // Save the updated state back to in-memory storage
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new InvalidOperationException("State must have a valid CorrelationId.");
            }

            await _stateStore.SaveStateAsync(new BonSagaState()
            {
                CorrelationId = correlationId,
                StateData = JsonSerializer.Serialize(state),
                Name = state.GetType().Name,
                Namespace = state.GetType().Namespace,
                LastUpdated = UtcNow
            });
        }
    }
}