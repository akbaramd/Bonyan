using Autofac;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IGlobalConfiguration = Hangfire.IGlobalConfiguration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonWorkerHangfireServiceCollectionExtensions
    {
        public static BonWorkerConfiguration AddHangfire(
            this BonWorkerConfiguration configure)
        {

            var pre = configure.Context.Services.GetPreConfigureActions<IGlobalConfiguration>();
            configure.Context.Services.AddHangfire(config =>
            {
                config.UseInMemoryStorage();
                var x = configure.Context.Services.GetObjectOrNull<IContainer>();
                config.UseAutofacActivator(x);
                pre.Configure(config);
            });

            configure.Context.Services.AddHangfireServer();

            configure.Context.Services.Replace(ServiceDescriptor.Singleton<IBonWorkerManager, HangfireWorkerManager>());


            return configure;
        }
    }
}