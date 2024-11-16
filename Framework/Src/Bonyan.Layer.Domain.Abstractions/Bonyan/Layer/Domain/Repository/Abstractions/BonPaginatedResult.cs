namespace Bonyan.Layer.Domain.Repository.Abstractions;

public class BonPaginatedResult<T> where T : class
{
    public IEnumerable<T> Results { get; }
    public int Skip { get; }
    public int Take { get; }
    public int Page { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Take);

    public BonPaginatedResult(IEnumerable<T> results, int skip, int take, int totalCount)
    {
        Results = results;
        Skip = skip;
        Take = take;
        Page = (skip / take) + 1;
        TotalCount = totalCount;
    }
}