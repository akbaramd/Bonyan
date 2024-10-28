namespace Bonyan.Layer.Domain.Model;

public class PaginatedResult<T> where T : class
{
  public IEnumerable<T> Results { get; set; } = new List<T>();
  public int Skip { get; set; }
  public int Take { get; set; }
  public int Page { get; set; }
  public int TotalCount { get; set; }
  public int TotalPages => (int)Math.Ceiling((double)TotalCount / Take);

  public PaginatedResult(IEnumerable<T> results, int skip, int take, int totalCount)
  {
    Results = results;
    Skip = skip;
    Take = take;
    Page = (skip / take) + 1;
    TotalCount = totalCount;
  }
}