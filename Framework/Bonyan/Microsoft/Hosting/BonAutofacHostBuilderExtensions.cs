using Autofac;
using Bonyan.Autofac;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Hosting;

public static class BonAutofacHostBuilderExtensions
{
  public static IHostBuilder UseBonAutofac(this IHostBuilder hostBuilder)
  {
    var containerBuilder = new ContainerBuilder();

    return hostBuilder.ConfigureServices((_, services) =>
      {
        services.AddObjectAccessor(containerBuilder);
      })
      .UseServiceProviderFactory(new BonAutofacServiceProviderFactory(containerBuilder));
  }
}
