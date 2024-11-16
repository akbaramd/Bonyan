using Bonyan.Messaging;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
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
            context.AddDomainLayer();
            return base.OnPostConfigureAsync(context);
        }
    }
}