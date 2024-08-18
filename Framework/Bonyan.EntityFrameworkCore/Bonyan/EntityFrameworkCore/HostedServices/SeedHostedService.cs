using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bonyan.EntityFrameworkCore.HostedServices;

public class SeedHostedService(IServiceProvider serviceProvider) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    using var scope = serviceProvider.CreateScope();
    var seeds = scope.ServiceProvider.GetServices<SeedBase>();

    foreach (var seed in seeds)
    {
      await seed.SeedAsync();
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
