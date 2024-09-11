namespace Bonyan.FastEndpoints;

public class FastEndpointConfiguration
{
  public IBonyanApplicationBuilder Builder {
    get;
    set;
  }

  public FastEndpointConfiguration(IBonyanApplicationBuilder builder)
  {
    Builder = builder;
  }

  
  
}
