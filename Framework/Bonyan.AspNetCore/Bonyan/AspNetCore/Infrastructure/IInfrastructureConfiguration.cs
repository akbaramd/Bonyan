using Bonyan.AspNetCore.Context;

namespace Bonyan.AspNetCore.Infrastructure;

public interface IInfrastructureConfiguration
{
  public IBonyanContext Context { get; set; }
  
}
