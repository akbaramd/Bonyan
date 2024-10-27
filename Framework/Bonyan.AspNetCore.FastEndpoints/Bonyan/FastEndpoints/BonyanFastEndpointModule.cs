using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Options;

namespace Bonyan.FastEndpoints;

[DependOn(typeof(BonyanAspNetCoreModule))]
public class BonyanFastEndpointModule : WebModule
{
  public override Task OnPreConfigureAsync(ModularityContext context)
  {
    context.Services.AddFastEndpoints();
    return base.OnPreConfigureAsync(context);
  }


  public override Task OnApplicationAsync(ModularityApplicationContext app)
  {
    var options = app.RequireService<IOptions<FastEndpointConfiguration>>();
    var fastEndpointConfiguration = options.Value;

    fastEndpointConfiguration?.InitBefore(app.WebApplication);
    app.WebApplication.UseFastEndpoints();
    fastEndpointConfiguration?.InitAfter(app.WebApplication);
    return base.OnApplicationAsync(app);
  }
}
