namespace Bonyan.Layer.Application.Dto;

public class BonPaginateDto : IBonPaginateDto
{
  public int Take { get; set; }
  public int Skip { get; set; }
}
public class BonFilterAndPaginateDto : BonPaginateDto
{
  public string? Search { get; set; }
}
