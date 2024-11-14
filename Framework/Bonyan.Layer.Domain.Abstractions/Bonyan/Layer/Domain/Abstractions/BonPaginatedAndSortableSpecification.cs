namespace Bonyan.Layer.Domain.Abstractions;

public abstract class BonPaginatedAndSortableSpecification<T> : BonPaginatedSpecification<T> where T : class
{
    protected BonPaginatedAndSortableSpecification(int skip, int take, string sortBy, string sortDirection)
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