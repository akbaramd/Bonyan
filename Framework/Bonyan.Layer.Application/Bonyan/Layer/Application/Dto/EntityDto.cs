namespace Bonyan.Layer.Application.Dto;

[Serializable]
public abstract class EntityDto : IEntityDto
{

}


[Serializable]
public abstract class EntityDto<TKey> : EntityDto, IEntityDto<TKey>
{

  public TKey Id { get; set; } = default!; 

 
}
