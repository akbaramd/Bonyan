using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Domain.Specifications;

public abstract class BonSpecification<T> : IBonSpecification<T> where T : class
{
  // The Handle method will be implemented by derived classes
  public abstract void Handle(IBonSpecificationContext<T> context);
}