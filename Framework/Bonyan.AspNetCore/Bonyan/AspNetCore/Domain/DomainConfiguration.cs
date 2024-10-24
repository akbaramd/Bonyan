using Bonyan.AspNetCore.Context;

namespace Bonyan.AspNetCore.Domain;

public class DomainConfiguration : IDomainConfiguration
{

  public DomainConfiguration(IBonyanContext applicationBuilder)
  {
    Context = applicationBuilder;
  }


  public void AddDomainService<TType,TImplement>() where TImplement : TType
  {
  
  }

  public IBonyanContext Context { get; set; }
}
