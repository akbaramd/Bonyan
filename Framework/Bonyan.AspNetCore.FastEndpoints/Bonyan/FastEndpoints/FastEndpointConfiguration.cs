namespace Bonyan.FastEndpoints;

public class FastEndpointConfiguration
{
  public FastEndpointConfiguration(IServiceCollection services, IConfiguration configuration)
  {
    Services = services;
    Configuration = configuration;
  }

  public IServiceCollection  Services { get;  }
  public IConfiguration  Configuration { get;  }
  
  
}
