using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers;

public class BonWorkersModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }


    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        context.AddWorkers(c =>
        {
            context.Services.ExecutePreConfiguredActions(c);
        });

        return base.OnPostConfigureAsync(context);
    }
}