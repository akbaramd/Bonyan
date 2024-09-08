using FastEndpoints;
using FastEndpoints.Swagger;

namespace Microsoft.AspNetCore.Builder;

public  static class FastEndpointsBonyanApplicationBuilderExtensions
{
  public static IBonyanApplicationBuilder AddFastEndpoints(this IBonyanApplicationBuilder applicationBuilder)
  {
    applicationBuilder.Services.AddFastEndpoints();
    applicationBuilder.Services.SwaggerDocument();
    

    applicationBuilder.AddConsoleMessage("FastEndpoints and Swagger registered","Extensions");
    return applicationBuilder;
  }
}
