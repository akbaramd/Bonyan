using Bonyan.AspNetCore.Modularity;

namespace Bonyan.FastEndpoints;

public  class FastEndpointsModule : Module
{
  
  public override void OnConfigure(IBonyanApplicationBuilder builder)
  {
    AddModule<FastEndpoints2Module>(c=>{});
    builder.AddFastEndpoints();
    base.OnConfigure(builder);
  }


  public override void OnBuild(BonyanApplication application)
  {
    application.UseFastEndpoints();
    base.OnBuild(application);
  }
}
