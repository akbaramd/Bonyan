using Autofac;
using Bonyan.Modularity;
using Hangfire;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers.Hangfire;

public class BonWorkersHangfireModule : BonWebModule
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

    public override Task OnPreApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        return base.OnPreApplicationAsync(webApplicationContext);
    }
}