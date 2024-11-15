using Autofac;
using Bonyan.AspNetCore.Job;
using Bonyan.Modularity;
using Bonyan.Worker;
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
        PreConfigure<BonWorkerConfiguration>(c => { c.AddHangfire(); });
        return base.OnConfigureAsync(context);
    }

    public override Task OnPreApplicationAsync(BonContext context)
    {
        return base.OnPreApplicationAsync(context);
    }
}