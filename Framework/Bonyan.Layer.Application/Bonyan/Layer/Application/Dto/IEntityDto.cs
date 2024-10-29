namespace Bonyan.Layer.Application.Dto;

public interface IEntityDto
{
}

public interface IEntityDto<TKey> : IEntityDto
{
  public TKey Id { get; set; }
}
