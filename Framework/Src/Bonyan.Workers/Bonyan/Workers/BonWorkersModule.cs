using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers;

public class BonWorkersModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        return base.OnConfigureAsync(context);
    }


    public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
    {
        context.AddWorkers(c =>
        {
            context.Services.ExecutePreConfiguredActions(c);
        });

        return base.OnPostConfigureAsync(context);
    }
}