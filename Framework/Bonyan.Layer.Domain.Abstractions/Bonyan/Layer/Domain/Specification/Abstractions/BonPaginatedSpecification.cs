namespace Bonyan.Layer.Domain.Specification.Abstractions;

public abstract class BonPaginatedSpecification<T> : IBonSpecification<T> where T : class
{
    protected BonPaginatedSpecification(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public int Take { get; set; }
    public int Skip { get; set; }
    // The Handle method will be implemented by derived classes
    public abstract void Handle(IBonSpecificationContext<T> context);
}