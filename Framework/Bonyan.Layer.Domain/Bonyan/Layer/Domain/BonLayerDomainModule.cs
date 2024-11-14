using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Messaging;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain
{
    public class BonLayerDomainModule : BonModule
    {
        public BonLayerDomainModule()
        {
            DependOn<BonMessagingModule>();
        }


        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            context.Services.AddBonDomainLayer();
            return base.OnPostConfigureAsync(context);
        }
    }
}