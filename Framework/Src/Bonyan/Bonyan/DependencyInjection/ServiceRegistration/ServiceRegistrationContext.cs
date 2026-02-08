using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IServiceRegistrationContext"/>.
/// </summary>
public sealed class ServiceRegistrationContext : IServiceRegistrationContext
{
    private readonly Dictionary<string, object?> _data = new();

    public IReadOnlyList<Assembly> Assemblies { get; }
    public IServiceCollection Services { get; }

    public ServiceRegistrationContext(IReadOnlyList<Assembly> assemblies, IServiceCollection services)
    {
        Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public object? GetData(string key) => _data.TryGetValue(key, out var v) ? v : null;
    public void SetData(string key, object? value) => _data[key] = value;
}
