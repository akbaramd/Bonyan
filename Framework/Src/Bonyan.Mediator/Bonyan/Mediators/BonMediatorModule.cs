using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class BonMediatorModule : BonModule
    {
        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            context.AddMediator();

            return base.OnPostConfigureAsync(context);
        }
    }
}