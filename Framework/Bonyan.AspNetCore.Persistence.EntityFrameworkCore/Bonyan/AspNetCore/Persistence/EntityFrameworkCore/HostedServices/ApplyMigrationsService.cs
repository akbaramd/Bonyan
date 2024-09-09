using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore.HostedServices;

public class ApplyMigrationsService<TContext>(IServiceProvider serviceProvider) : IHostedService
  where TContext : DbContext
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TContext>();
    await context.Database.MigrateAsync(cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
