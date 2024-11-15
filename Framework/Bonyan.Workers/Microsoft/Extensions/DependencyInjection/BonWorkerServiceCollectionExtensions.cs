using Bonyan.Modularity;
using Bonyan.Workers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonWorkerServiceCollectionExtensions
    {
        public static BonConfigurationContext AddWorkers(
            this BonConfigurationContext configurationContext,
            Action<BonWorkerConfiguration> configureOptions)
        {
            var options = new BonWorkerConfiguration(configurationContext);

            configureOptions(options);

            if (configurationContext.Services.All(service => service.ServiceType != typeof(IBonWorkerManager)))
            {
                configurationContext.Services.AddSingleton<IBonWorkerManager, InMemoryBonWorkerManager>();
            }

            configurationContext.Services.AddHostedService<BonWorkerHostedService>();
            return configurationContext;
        }
    }
}