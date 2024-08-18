﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bonyan.EntityFrameworkCore.HostedServices;

public class EnsureDatabaseCreatedService<TContext>(IServiceProvider serviceProvider) : IHostedService
  where TContext : DbContext
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TContext>();
    await context.Database.EnsureCreatedAsync(cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
