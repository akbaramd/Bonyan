using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using SharedKernel.ExceptionHandling.Middlewares;

namespace Bonyan.AspNetCore;

public class BonyanAspNetCoreModule : WebModule
{
  public override Task OnPreApplicationAsync(ModularityApplicationContext app)
  {
    app.WebApplication.UseMiddleware<ExceptionHandlingMiddleware>();
    return base.OnPreApplicationAsync(app);
  }
}
