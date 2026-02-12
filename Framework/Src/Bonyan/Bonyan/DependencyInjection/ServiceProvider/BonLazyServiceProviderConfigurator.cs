using System;
using System.IO;
using System.Text.Json;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Implementation of <see cref="IBonLazyServiceProviderConfigurator"/> for managing and accessing services within a lazy-loaded service provider context.
/// </summary>
public class BonLazyServiceProviderConfigurator : IBonLazyServiceProviderConfigurator
{
    private const string AgentLogPath = @"D:\Projects\Bonyan2\.cursor\debug.log";

    /// <summary>
    /// Injected by the container as <see cref="Lazy{T}"/> to avoid circular dependency (e.g. when resolving <see cref="IBonLazyServiceProvider"/> during construction).
    /// </summary>
    private IBonLazyServiceProvider _lazyServiceProvider = default!;

    public IBonLazyServiceProvider LazyServiceProvider
    {
        get => _lazyServiceProvider;
        set
        {
#region agent log
            try
            {
                try
                {
                    Console.Error.WriteLine(
                        $"[agent-log] BonLazyServiceProviderConfigurator.LazyServiceProvider set reached | asm={typeof(BonLazyServiceProviderConfigurator).Assembly.Location} | assignedType={value?.GetType().FullName}");
                }
                catch
                {
                    // ignore
                }

                var dir = Path.GetDirectoryName(AgentLogPath);
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.AppendAllText(
                    AgentLogPath,
                    JsonSerializer.Serialize(new
                    {
                        id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}",
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        runId = "pre-fix",
                        hypothesisId = "H2",
                        location = "BonLazyServiceProviderConfigurator.cs:LazyServiceProvider:set",
                        message = "Property injection assigned LazyServiceProvider",
                        data = new
                        {
                            assignedType = value?.GetType().FullName
                        }
                    }) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                try
                {
                    Console.Error.WriteLine($"[agent-log-write-failed] BonLazyServiceProviderConfigurator.cs:LazyServiceProvider:set :: {ex.GetType().FullName} :: {ex.Message}");
                }
                catch
                {
                    // ignore
                }
            }
#endregion
            _lazyServiceProvider = value;
        }
    }


    /// <inheritdoc />
    public object? GetService(Type serviceType) => LazyServiceProvider.GetService(serviceType);

    /// <inheritdoc />
    public T GetService<T>(T defaultValue) => LazyServiceProvider.GetService(defaultValue);

    /// <inheritdoc />
    public object GetService(Type serviceType, object defaultValue) => LazyServiceProvider.GetService(serviceType, defaultValue);

    /// <inheritdoc />
    public T GetService<T>(Func<IServiceProvider, object> factory) => LazyServiceProvider.GetService<T>(factory);

    /// <inheritdoc />
    public object GetService(Type serviceType, Func<IServiceProvider, object> factory) => LazyServiceProvider.GetService(serviceType, factory);

    /// <inheritdoc />
    public T LazyGetRequiredService<T>() => LazyServiceProvider.LazyGetRequiredService<T>();

    /// <inheritdoc />
    public object LazyGetRequiredService(Type serviceType) => LazyServiceProvider.LazyGetRequiredService(serviceType);

    /// <inheritdoc />
    public T? LazyGetService<T>() => LazyServiceProvider.LazyGetService<T>();

    /// <inheritdoc />
    public object? LazyGetService(Type serviceType) => LazyServiceProvider.LazyGetService(serviceType);

    /// <inheritdoc />
    public T LazyGetService<T>(T defaultValue) => LazyServiceProvider.LazyGetService(defaultValue);

    /// <inheritdoc />
    public object LazyGetService(Type serviceType, object defaultValue) => LazyServiceProvider.LazyGetService(serviceType, defaultValue);

    /// <inheritdoc />
    public object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory) => LazyServiceProvider.LazyGetService(serviceType, factory);

    /// <inheritdoc />
    public T LazyGetService<T>(Func<IServiceProvider, object> factory) => LazyServiceProvider.LazyGetService<T>(factory);
}
