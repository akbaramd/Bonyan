using Bonyan;
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Bonyan.Tracing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonServiceCollectionExtensions
    {
        public static IServiceCollection AddBonyan(
            
            this IServiceCollection services, string serviceKey,Action<BonConfigurationContext> configure)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceKey, nameof(serviceKey));
            
            services.AddTransient(typeof(BonAsyncDeterminationInterceptor<>));
            services.AddTransient<IBonCachedServiceProviderBase, BonLazyServiceProvider>();
            services.AddTransient<IBonLazyServiceProvider, BonLazyServiceProvider>();
            services.AddSingleton<ICorrelationIdProvider, DefaultCorrelationIdProvider>();
            services.AddOptions();
            services.AddLogging();
            services.AddLocalization();
            
            var context = new BonConfigurationContext(services);
            context.ServiceManager = new BonServiceManager()
            {
                ServiceId = serviceKey
            };
            configure.Invoke(context);

            services.AddSingleton(context.ServiceManager);
            
            return services;
        }
    }
}


