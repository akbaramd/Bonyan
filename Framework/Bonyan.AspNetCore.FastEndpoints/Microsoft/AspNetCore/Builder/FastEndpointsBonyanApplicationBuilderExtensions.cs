using Bonyan.FastEndpoints;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace Microsoft.AspNetCore.Builder;

public  static class FastEndpointsBonyanApplicationBuilderExtensions
{
  public static IBonyanApplicationBuilder AddFastEndpoints(this IBonyanApplicationBuilder applicationBuilder,Action<FastEndpointConfiguration>? action = null)
  {
    applicationBuilder.GetServicesCollection().AddFastEndpoints();
    applicationBuilder.GetServicesCollection().SwaggerDocument();
    
    action?.Invoke(new FastEndpointConfiguration(applicationBuilder));


    applicationBuilder.AddConsoleMessage("FastEndpoints and Swagger registered","Extensions");
    return applicationBuilder;
  }
}
