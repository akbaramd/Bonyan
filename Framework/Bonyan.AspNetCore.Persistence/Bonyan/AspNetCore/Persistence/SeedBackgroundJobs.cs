using Bonyan.AspNetCore.Jobs;
using Bonyan.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence;

public class SeedBackgroundJobs(IServiceProvider serviceProvider) : IJob
{
  public async Task ExecuteAsync()
  {
    using var scope = serviceProvider.CreateScope();
    var seeds = scope.ServiceProvider.GetServices<SeedBase>();

    foreach (var seed in seeds)
    {
      await seed.SeedAsync();
    }
  }
}
