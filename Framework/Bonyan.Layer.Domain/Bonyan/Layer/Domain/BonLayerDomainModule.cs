using Bonyan.Layer.Domain.Events;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain
{
    public class BonLayerDomainModule : BonModule
    {
        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            var services = context.Services;
            
            // Register the InMemoryDomainEventDispatcher as a singleton
            services.AddSingleton<IBonDomainEventDispatcher, BonInMemoryBonDomainEventDispatcher>();

            return base.OnConfigureAsync(context);
        }
    }
}
