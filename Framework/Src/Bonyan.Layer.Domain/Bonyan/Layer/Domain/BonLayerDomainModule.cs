using Bonyan.Mediators;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain
{
    public class BonLayerDomainModule : BonModule
    {
        public BonLayerDomainModule()
        {
            DependOn<BonMediatorModule>();
        }


        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            context.AddDomain();
            return base.OnPostConfigureAsync(context);
        }
    }
}