namespace Bonyan.DependencyInjection;

/// <summary>
/// Context passed when exposing service types (e.g. for proxy or discovery).
/// </summary>
public interface IOnServiceExposingContext
{
    /// <summary>Implementation type being exposed.</summary>
    Type ImplementationType { get; }

    /// <summary>Types (e.g. interfaces) this implementation is exposed as.</summary>
    List<Type> ExposedTypes { get; }
}