using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Saga;
using Bonyan.StateMachine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using static System.DateTime;

namespace Bonyan.Messaging.RabbitMQ.HostedServices
{
    public class RabbitMqSagaBackgroundService : BackgroundService
    {
        private readonly IBonMessageBus _messageBus;
        private readonly IBonMessageSubscriber _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSagaTypeAccessor _sagaTypeAccessor;
        private readonly IBonStateStore _stateStore;
        private readonly ILogger<RabbitMqSagaBackgroundService> _logger;

        public RabbitMqSagaBackgroundService(
            IBonMessageBus messageBus,
            IServiceProvider serviceProvider,
            RabbitMQSagaTypeAccessor sagaTypeAccessor,
            IBonStateStore stateStore,
            IBonMessageSubscriber messageSubscriber,
            ILogger<RabbitMqSagaBackgroundService> logger)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _sagaTypeAccessor = sagaTypeAccessor ?? throw new ArgumentNullException(nameof(sagaTypeAccessor));
            _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
            _messageSubscriber = messageSubscriber ?? throw new ArgumentNullException(nameof(messageSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMqSagaBackgroundService started.");

            // Register all saga events
            foreach (var registration in _sagaTypeAccessor.GetAllSagas())
            {
                var sagaType = registration.SagaType;
                var instanceType = registration.InstanceType;
                var queueName = registration.QueueName;

                RegisterSagaEventsGeneric(queueName, sagaType, instanceType, stoppingToken);
            }

            // Wait until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);

            _logger.LogInformation("RabbitMqSagaBackgroundService is stopping.");
        }

        private void RegisterSagaEventsGeneric(string queueName, Type sagaType, Type instanceType, CancellationToken stoppingToken)
        {
            var registerMethod = GetType()
                .GetMethod(nameof(RegisterSagaEvents), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(sagaType, instanceType);

            registerMethod?.Invoke(this, new object[] { queueName, stoppingToken });
        }

        private void RegisterSagaEvents<TSaga, TInstance>(string queueName, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            using var scope = _serviceProvider.CreateScope();
            var saga = scope.ServiceProvider.GetRequiredService<TSaga>();

            _logger.LogInformation($"Subscribed to saga events for {typeof(TSaga).Name} on queue {queueName}.");

            foreach (var eventType in saga.DefinedEvents)
            {
                SubscribeToSagaEventGeneric<TSaga, TInstance>(queueName, saga, eventType, stoppingToken);
            }
        }

        private void SubscribeToSagaEventGeneric<TSaga, TInstance>(string queueName, TSaga saga, Type eventType, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            var subscribeMethod = GetType()
                .GetMethod(nameof(SubscribeToSagaEvent), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(typeof(TSaga), typeof(TInstance), eventType);

            subscribeMethod?.Invoke(this, new object[] { queueName, saga, stoppingToken });
        }

        private async Task SubscribeToSagaEvent<TSaga, TInstance, TEvent>(string queueName, TSaga saga, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
            where TEvent : class
        {
            _messageSubscriber.Subscribe<TEvent>($"saga.{queueName}.{typeof(TEvent).Name}", async context =>
            {
                if (stoppingToken.IsCancellationRequested) return;

                try
                {
                    _logger.LogInformation($"Processing event {typeof(TEvent).Name} for saga {queueName}.");
                    var state = await LoadSagaStateAsync<TSaga, TInstance>(saga, context.CorrelationId, stoppingToken);
                    await saga.ProcessAsync(state, context, context.Message);
                    await SaveSagaStateAsync(saga, state, context.CorrelationId, stoppingToken);
                    _logger.LogInformation($"Processed event {typeof(TEvent).Name} successfully for saga {queueName}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing event {typeof(TEvent).Name} for saga {queueName}.");
                }
            });
        }

        private async Task<TInstance> LoadSagaStateAsync<TSaga, TInstance>(TSaga saga, string correlationId, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            _logger.LogInformation($"Loading saga state for CorrelationId {correlationId}.");

            var stateData = await _stateStore.LoadStateAsync(correlationId);
            return stateData != null
                ? JsonConvert.DeserializeObject<TInstance>(stateData.StateData) ?? new TInstance()
                : new TInstance();
        }

        private async Task SaveSagaStateAsync<TSaga, TInstance>(TSaga saga, TInstance state, string correlationId, CancellationToken stoppingToken)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance
        {
            if (string.IsNullOrWhiteSpace(correlationId))
                throw new InvalidOperationException("State must have a valid CorrelationId.");

            _logger.LogInformation($"Saving saga state for CorrelationId {correlationId}.");

            await _stateStore.SaveStateAsync(new BonSagaState
            {
                CorrelationId = correlationId,
                StateData = JsonConvert.SerializeObject(state),
                Name = state.GetType().Name,
                Namespace = state.GetType().Namespace,
                LastUpdated = UtcNow
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMqSagaBackgroundService.");

            // Gracefully handle shutdown and dispose of resources
            await base.StopAsync(cancellationToken);

            // Dispose the message subscriber
            _messageSubscriber.Dispose();

            _logger.LogInformation("RabbitMqSagaBackgroundService stopped.");
        }
    }
}
