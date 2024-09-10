namespace Bonyan.DomainDrivenDesign.Domain.Entities;



/// <summary>
/// Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class DefinitionEntity : Entity<string>
{
  public string Title { get; set; } = string.Empty;
}

