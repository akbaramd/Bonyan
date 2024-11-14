using Bonyan.Layer.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Query;

namespace Bonyan.Layer.Domain.Specifications;

public class BonIncludeBonSpecificationContext<T, TProperty> : BonSpecificationContext<T>, IBonIncludeBonSpecificationContext<T, TProperty> where T : class
{
  public new IIncludableQueryable<T,TProperty> Query { get; set; }
  
  public BonIncludeBonSpecificationContext(IIncludableQueryable<T,TProperty> query) : base(query) {
    Query = query;
  }


}
