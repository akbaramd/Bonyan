using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediators
{
    public class BonMediatorModule : BonModule
    {
        public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)  
        {
            
            context.AddMediator(c =>
            {
                context.Services.ExecutePreConfiguredActions(c);
            });

            return base.OnConfigureAsync(context, cancellationToken);
        }
    }
}