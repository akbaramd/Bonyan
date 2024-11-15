using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Worker;

public class BonWorkersModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }


    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        var preConfigure = context.Services.GetPreConfigureActions<BonWorkerConfiguration>();
        context.Services.AddBonWorkers(c =>
        {
            preConfigure.Configure(c);
        });

        return base.OnPostConfigureAsync(context);
    }
}