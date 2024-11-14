namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonSpecification<T> where T : class
{
  void Handle(IBonSpecificationContext<T> context);
}