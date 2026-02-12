using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Extensions.DependencyInjection;

/// <summary>
/// Extensions for working with <see cref="ServiceDescriptor"/> (keyed vs non-keyed).
/// </summary>
internal static class ServiceDescriptorExtensions
{
    /// <summary>
    /// Normalizes the implementation instance between keyed and non-keyed services.
    /// </summary>
    public static object? NormalizedImplementationInstance(this ServiceDescriptor descriptor) =>
        descriptor.IsKeyedService ? descriptor.KeyedImplementationInstance : descriptor.ImplementationInstance;

    /// <summary>
    /// Normalizes the implementation type between keyed and non-keyed services.
    /// </summary>
    public static Type? NormalizedImplementationType(this ServiceDescriptor descriptor) =>
        descriptor.IsKeyedService ? descriptor.KeyedImplementationType : descriptor.ImplementationType;
}
