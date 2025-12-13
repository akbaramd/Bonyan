using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Bonyan.Mediators
{
    public class InMemoryBonMediator : IBonMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryBonMediator> _logger;

        public InMemoryBonMediator(IServiceProvider serviceProvider, ILogger<InMemoryBonMediator> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Private Methods for Internal Logic
        private async Task<TResponse> HandleCommandWithResponseAsync<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : IBonCommand<TResponse>
        {
            _logger.LogInformation("Handling command of type {CommandType}", command.GetType().Name);

            var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand, TResponse>>().ToList();
            var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand, TResponse>>();
            if (handler == null)
            {
                _logger.LogError("No handler found for command type {CommandType}", command.GetType().Name);
                throw new InvalidOperationException($"No Handler found for command type {command.GetType().Name}");
            }

            async Task<TResponse> HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

            var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand, TResponse>>()
                .Aggregate((Func<Task<TResponse>>)HandlerDelegate,
                    (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

            var response = await pipeline();
            _logger.LogInformation("Successfully handled command of type {CommandType}", command.GetType().Name);
            return response;
        }

        private async Task HandleCommandAsync<TCommand>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : IBonCommand
        {
            _logger.LogInformation("Handling command of type {CommandType}", command.GetType().Name);

            var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand>>().ToList();
            var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand>>();
            if (handler == null)
            {
                _logger.LogError("No handler found for command type {CommandType}", command.GetType().Name);
                throw new InvalidOperationException($"No Handler found for command type {command.GetType().Name}");
            }

            async Task HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

            var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand>>()
                .Aggregate((Func<Task>)HandlerDelegate,
                    (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

            await pipeline();
            _logger.LogInformation("Successfully handled command of type {CommandType}", command.GetType().Name);
        }

        private async Task HandleEventAsync<TEvent>(
            TEvent eventMessage,
            CancellationToken cancellationToken = default)
            where TEvent : IBonEvent
        {
            _logger.LogInformation("Publishing event of type {EventType}", eventMessage.GetType().Name);

            var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TEvent>>().ToList();
            var handlers = _serviceProvider.GetServices<IBonEventHandler<TEvent>>().ToList();
            if (!handlers.Any())
            {
                _logger.LogWarning("No event handlers found for event type {EventType}", eventMessage.GetType().Name);
                return;
            }

            // Pipeline for publishing events
            async Task HandlerDelegate()
            {
                var tasks = handlers.Select(handler => handler.HandleAsync(eventMessage, cancellationToken));
                await Task.WhenAll(tasks);
            }

            var pipeline = behaviors.Reverse<IBonMediatorBehavior<TEvent>>()
                .Aggregate((Func<Task>)HandlerDelegate,
                    (next, behavior) => () => behavior.HandleAsync(eventMessage, next, cancellationToken));

            await pipeline();
            _logger.LogInformation("Successfully published event of type {EventType}", eventMessage.GetType().Name);
        }

        // Public Methods Using Reflection to Call Private Methods
        public async Task<TResponse> SendAsync<TResponse>(IBonCommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            var method = GetType().GetMethod(nameof(HandleCommandWithResponseAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(command.GetType(), typeof(TResponse));

            if (method == null)
                throw new InvalidOperationException($"Unable to find the method {nameof(HandleCommandWithResponseAsync)}");

            return await (Task<TResponse>)method.Invoke(this, new object[] { command, cancellationToken });
        }

        public async Task SendAsync(IBonCommand command, CancellationToken cancellationToken = default)
        {
            var method = GetType().GetMethod(nameof(HandleCommandAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(command.GetType());

            if (method == null)
                throw new InvalidOperationException($"Unable to find the method {nameof(HandleCommandAsync)}");

            await (Task)method.Invoke(this, new object[] { command, cancellationToken });
        }


        public async Task PublishAsync(IBonEvent eventMessage, CancellationToken cancellationToken = default)
        {
            var method = GetType().GetMethod(nameof(HandleEventAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(eventMessage.GetType());

            if (method == null)
                throw new InvalidOperationException($"Unable to find the method {nameof(HandleEventAsync)}");

            await (Task)method.Invoke(this, new object[] { eventMessage, cancellationToken });
        }
    }
}
