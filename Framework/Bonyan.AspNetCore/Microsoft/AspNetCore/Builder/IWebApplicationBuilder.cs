

namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationBuilder
{
  public IServiceCollection Services { get; }
  public ConfigureHostBuilder Host { get; }
  public IConfiguration Configuration { get; }


 
 public IServiceCollection GetServicesCollection() => Services;
 public IConfiguration GetConfiguration() => Configuration;
 

  BonyanApplication Build();
}
