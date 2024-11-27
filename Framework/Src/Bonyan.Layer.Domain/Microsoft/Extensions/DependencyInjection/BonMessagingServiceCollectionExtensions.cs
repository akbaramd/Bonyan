using Bonyan.Exceptions;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonDomainServiceCollectionExtensions
    {
        public static BonConfigurationContext AddDomainLayer(this BonConfigurationContext context)
        {
            // Register the in-memory domain event bus by default
            context.Services.AddSingleton<IBonDomainEventDispatcher, BonDomainEventDispatcher>();

            return context;
        }

       
    }
}