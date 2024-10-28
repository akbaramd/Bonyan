
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Figgle;

namespace Microsoft.AspNetCore.Builder;

public class BonyanApplication(WebApplication application, BonyanServiceInfo serviceInfo)
{
  public WebApplication Application => application;
  public BonyanServiceInfo ServiceInfo { get; } = serviceInfo;

  public static IBonyanApplicationBuilder CreateApplicationBuilder<TModule>(
    string[] args) where TModule : IModule
  {
    var applicationBuilder = WebApplication.CreateBuilder(args);

    var modularApp = new WebModularityApplication<TModule>(applicationBuilder.Services);
    applicationBuilder.Services.AddSingleton<IModularityApplication>(modularApp);
    applicationBuilder.Services.AddSingleton<IWebModularityApplication>(modularApp);
    
    var builder = new BonyanApplicationBuilder(modularApp,applicationBuilder);
    return builder;
  }

}
