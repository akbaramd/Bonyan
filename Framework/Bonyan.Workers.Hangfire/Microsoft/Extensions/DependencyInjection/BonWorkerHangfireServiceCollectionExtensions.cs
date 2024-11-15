using Autofac;
using AutoMapper.Internal;
using Bonyan.AspNetCore.Job;
using Bonyan.Worker;
using Bonyan.Workers.Hangfire;
using Hangfire.AspNetCore;
using Hangfire;
using IGlobalConfiguration = Hangfire.IGlobalConfiguration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonWorkerHangfireServiceCollectionExtensions
    {
        public static BonWorkerConfiguration AddHangfire(
            this BonWorkerConfiguration context)
        {

            context.UseWorkerManager<HangfireWorkerManager>();
            var preActions = context.Services.GetPreConfigureActions<IGlobalConfiguration>();

            context.Services.AddHangfire(config =>
            {
                config.UseInMemoryStorage();
                var x = context.Services.GetObjectOrNull<IContainer>();
                config.UseAutofacActivator(x);
                preActions.Configure(config);
            });

            context.Services.AddHangfireServer();
            
            return context;
        }
    }
}
