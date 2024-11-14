namespace Bonyan.Layer.Application.Dto;

[Serializable]
public abstract class BonEntityDto : IBonEntityDto
{

}


[Serializable]
public abstract class BonEntityDto<TKey> : BonEntityDto, IBonEntityDto<TKey>
{

  public TKey Id { get; set; } = default!; 

 
}
