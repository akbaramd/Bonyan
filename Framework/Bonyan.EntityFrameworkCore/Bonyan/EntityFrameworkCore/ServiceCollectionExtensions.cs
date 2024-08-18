using Bonyan.EntityFrameworkCore.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services,
    Action<EfCoreConfiguration> configure)
  {
    var configuration = new EfCoreConfiguration(services);
    configure(configuration);


    // Optionally run seeds after the app starts
    services.AddHostedService<SeedHostedService>();

    return services;
  }
}
