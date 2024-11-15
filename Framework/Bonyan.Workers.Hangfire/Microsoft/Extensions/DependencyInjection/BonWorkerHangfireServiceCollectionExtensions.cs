using Autofac;
using AutoMapper.Internal;
using Bonyan.Workers;
using Bonyan.Workers.Hangfire;
using Hangfire.AspNetCore;
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
            var preActions = configure.Context.Services.GetPreConfigureActions<IGlobalConfiguration>();

            configure.Context.Services.AddHangfire(config =>
            {
                config.UseInMemoryStorage();
                var x = configure.Context.Services.GetObjectOrNull<IContainer>();
                config.UseAutofacActivator(x);
                preActions.Configure(config);
            });

            configure.Context.Services.AddHangfireServer();

            configure.Context.Services.Replace(ServiceDescriptor.Singleton<IBonWorkerManager, HangfireWorkerManager>());


            return configure;
        }
    }
}