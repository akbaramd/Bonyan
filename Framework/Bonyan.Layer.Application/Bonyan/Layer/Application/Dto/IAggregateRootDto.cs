namespace Bonyan.Layer.Application.Dto;

public interface IAggregateRootDto : IEntityDto
{
}

public interface IAggregateRootDto<TKey> : IAggregateRootDto, IEntityDto<TKey>
{
}
