namespace Dto;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class BonAggregateRootDto : BonEntityDto, IBonAggregateRootDto
{
 
}
/// <summary>
/// Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class BonAggregateRootDto<TKey> : BonEntityDto<TKey>, IBonAggregateRootDto<TKey>
{
  
}