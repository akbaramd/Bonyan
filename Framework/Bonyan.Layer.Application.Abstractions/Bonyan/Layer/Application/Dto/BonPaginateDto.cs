namespace Bonyan.Layer.Application.Dto;

public class BonPaginateDto
{
  public int Take { get; set; }
  public int Skip { get; set; }
}
public class FilterAndBonPaginateDto : BonPaginateDto
{
  public string? Search { get; set; }
}
