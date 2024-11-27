using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediators
{
    public class BonMediatorModule : BonModule
    {
        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            
            context.AddMediator(c =>
            {
                context.Services.ExecutePreConfiguredActions(c);
            });

            return base.OnPostConfigureAsync(context);
        }
    }
}