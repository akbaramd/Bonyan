using Bonyan.AspNetCore.Context;

namespace Bonyan.AspNetCore.Infrastructure;

public class InfrastructureConfiguration : IInfrastructureConfiguration
{

  public InfrastructureConfiguration(IBonyanContext applicationBuilder)
  {
    Context = applicationBuilder;
  }

  public IBonyanContext Context { get; set; }
}
