namespace Bonyan.Layer.Application.Dto;

public interface IBonEntityDto
{
}

public interface IBonEntityDto<TKey> : IBonEntityDto
{
  public TKey Id { get; set; }
}
