namespace Bonyan.DomainDrivenDesign.Domain.Specifications;

public interface ISpecification<T> where T : class
{
  void Handle(ISpecificationContext<T> context);
}