namespace Bonyan.Layer.Domain.Entities;

/// <summary>
///     Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class BonDefinitionEntity<TKey> : BonEntity<TKey>
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}