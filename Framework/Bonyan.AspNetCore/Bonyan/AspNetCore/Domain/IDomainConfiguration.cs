using Bonyan.AspNetCore.Context;

namespace Bonyan.AspNetCore.Domain;

public interface IDomainConfiguration
{
  public IBonyanContext Context { get; set; }
}
