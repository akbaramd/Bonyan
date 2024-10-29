using Autofac;
using Bonyan.IoC.Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class BonyanAutofacHostBuilderExtensions
{
  public static IHostBuilder UseAutofac(this IHostBuilder hostBuilder)
  {
    var containerBuilder = new ContainerBuilder();

    return hostBuilder.ConfigureServices((_, services) =>
      {
        services.AddObjectAccessor(containerBuilder);
      })
      .UseServiceProviderFactory(new BonyanAutofacServiceProviderFactory(containerBuilder));
  }
}
