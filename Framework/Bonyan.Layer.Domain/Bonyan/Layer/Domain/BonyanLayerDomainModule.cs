using Bonyan.Layer.Domain.Events;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.Layer.Domain
{
    public class BonyanLayerDomainModule : Module
    {
        public override Task OnConfigureAsync(ServiceConfigurationContext context)
        {
            var services = context.Services;
            
            // Register the InMemoryDomainEventDispatcher as a singleton
            services.AddSingleton<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();

            return base.OnConfigureAsync(context);
        }
    }
}
