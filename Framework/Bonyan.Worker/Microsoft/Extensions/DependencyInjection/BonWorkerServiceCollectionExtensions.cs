using Bonyan.AspNetCore.Job;
using Bonyan.Worker;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddBonWorkers(
            this IServiceCollection services,
            Action<BonWorkerConfiguration> configureOptions)
        {
            var options = new BonWorkerConfiguration(services);

            options.UseWorkerManager<InMemoryBonWorkerManager>();
            
            configureOptions(options);

            // Register workers
            options.RegisterWorkerManager();
            options.RegisterWorkers();
            
            services.AddHostedService<BonWorkerHostedService>();
            return services;
        }
    }
}

