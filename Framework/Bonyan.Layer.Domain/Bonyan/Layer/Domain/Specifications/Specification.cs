namespace Bonyan.Layer.Domain.Specifications;

public abstract class Specification<T> : ISpecification<T> where T : class
{
  // The Handle method will be implemented by derived classes
  public abstract void Handle(ISpecificationContext<T> context);
}


public abstract class PaginatedSpecification<T> : ISpecification<T> where T : class
{
  protected PaginatedSpecification(int skip, int take)
  {
    Skip = skip;
    Take = take;
  }

  public int Take { get; set; }
  public int Skip { get; set; }
  // The Handle method will be implemented by derived classes
  public abstract void Handle(ISpecificationContext<T> context);
}



public abstract class PaginatedAndSortableSpecification<T> : PaginatedSpecification<T> where T : class
{
  protected PaginatedAndSortableSpecification(int skip, int take, string sortBy, string sortDirection)
    : base(skip, take)
  {
    // Validate that SortBy is not null or empty
    if (string.IsNullOrWhiteSpace(sortBy))
    {
      throw new ArgumentException("SortBy cannot be null or empty.", nameof(sortBy));
    }

    SortBy = sortBy;

    // Ensure that SortDirection is either "asc" or "desc", defaulting to "asc"
    SortDirection = sortDirection != null && sortDirection.ToLower() == "desc" ? "desc" : "asc";
  }

  public string SortBy { get; set; }
  public string SortDirection { get; set; }
}