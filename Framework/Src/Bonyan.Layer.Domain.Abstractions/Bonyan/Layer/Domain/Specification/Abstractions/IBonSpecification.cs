namespace Bonyan.Layer.Domain.Specification.Abstractions;

public interface IBonSpecification<T> where T : class
{
  void Handle(IBonSpecificationContext<T> context);
}