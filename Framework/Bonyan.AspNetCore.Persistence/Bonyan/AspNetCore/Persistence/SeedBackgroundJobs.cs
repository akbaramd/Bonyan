using Bonyan.AspNetCore.Jobs;

namespace Bonyan.AspNetCore.Persistence
{
    public class SeedBackgroundJobs : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedBackgroundJobs(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
          using var scope = _serviceProvider.CreateScope();
          var seeds = scope.ServiceProvider.GetServices<ISeeder>();

          foreach (var seed in seeds)
          {
            await seed.SeedAsync(cancellationToken);
          }
        }

    }
}
