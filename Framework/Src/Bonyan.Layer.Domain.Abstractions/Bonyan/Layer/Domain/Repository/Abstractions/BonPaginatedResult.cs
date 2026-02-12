namespace Bonyan.Layer.Domain.Repository.Abstractions;

public class BonPaginatedResult<T> 
{
    public IEnumerable<T> Results { get; }
    public int Skip { get; }
    public int Take { get; }
    public int Page { get; }
    public int TotalCount { get; }
    public int TotalPages => Take <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / Take);

    public BonPaginatedResult(IEnumerable<T> results, int skip, int take, int totalCount)
    {
        Results = results;
        Skip = skip;
        Take = take <= 0 ? 0 : take;
        Page = Take <= 0 ? 1 : (Skip / Take) + 1;
        TotalCount = totalCount;
    }
}