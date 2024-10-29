namespace Bonyan.Layer.Application.Dto;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class AggregateRootDto : EntityDto, IAggregateRootDto
{
 
}
/// <summary>
/// Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class AggregateRootDto<TKey> : EntityDto<TKey>, IAggregateRootDto<TKey>
{
  
}