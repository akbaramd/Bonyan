using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain.Events
{
    /// <summary>
    /// In-memory implementation of IDomainEventDispatcher, useful for testing and lightweight scenarios.
    /// </summary>
    public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDomainEventDispatcher"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for resolving event handlers.</param>
        public InMemoryDomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        /// <summary>
        /// Dispatches a domain event to the appropriate handlers.
        /// </summary>
        /// <param name="domainEvent">The domain event to dispatch.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent

        {
            var handler = _serviceProvider.GetService<IDomainEventHandler<TEvent>>();
            if (handler != null)
            {
                await handler.Handle(domainEvent);
            }
        }
    }
}