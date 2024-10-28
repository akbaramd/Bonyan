using Microsoft.EntityFrameworkCore.Query;

namespace Bonyan.DomainDrivenDesign.Domain.Specifications;

public class IncludeSpecificationContext<T, TProperty> : SpecificationContext<T>, IIncludeSpecificationContext<T, TProperty> where T : class
{
  public new IIncludableQueryable<T,TProperty> Query { get; set; }
  
  public IncludeSpecificationContext(IIncludableQueryable<T,TProperty> query) : base(query) {
    Query = query;
  }


}
