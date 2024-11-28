using Autofac;
using Bonyan;
using Bonyan.Modularity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonServiceCollectionExtensions
    {
        public static IServiceCollection AddBonyan(
            
            this IServiceCollection services, string serviceName,Action<BonConfigurationContext> configure)
        {
            var context = new BonConfigurationContext(services);
            context.ServiceManager = new BonServiceManager()
            {
                ServiceId = serviceName
            };
            configure.Invoke(context);

            services.AddSingleton(context.ServiceManager);
            
            return services;
        }
    }
}


