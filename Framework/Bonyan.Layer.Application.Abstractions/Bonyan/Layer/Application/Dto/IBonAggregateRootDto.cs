namespace Dto;

public interface IBonAggregateRootDto : IBonEntityDto
{
}

public interface IBonAggregateRootDto<TKey> : IBonAggregateRootDto, IBonEntityDto<TKey>
{
}
