namespace Bonyan.Layer.Application.Dto;

public class PaginateDto
{
  public int Take { get; set; }
  public int Skip { get; set; }
}
public class FilterAndPaginateDto : PaginateDto
{
  public string? Search { get; set; }
}
