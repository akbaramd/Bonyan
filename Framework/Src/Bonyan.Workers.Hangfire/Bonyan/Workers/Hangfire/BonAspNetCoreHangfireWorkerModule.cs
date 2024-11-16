using Autofac;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Hangfire;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers.Hangfire;

public class BonWorkersHangfireModule : BonModule
{
    public BonWorkersHangfireModule()
    {
        DependOn<BonWorkersModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<BonWorkerConfiguration>(c =>
        {
            c.AddHangfire();
        });
        return base.OnConfigureAsync(context);
    }

}