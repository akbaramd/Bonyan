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
        var preConfigure = context.Services.GetPreConfigureActions<BonWorkerConfiguration>();

        context.AddWorkers(c =>
        {
            preConfigure.Configure(c);
        });

        return base.OnPostConfigureAsync(context);
    }
}