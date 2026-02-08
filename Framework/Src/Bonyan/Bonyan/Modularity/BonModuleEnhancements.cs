using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Enhanced base module class with common helper methods and properties.
/// Provides logging, configuration, health checks, and other common functionality.
/// </summary>
public abstract class BonModuleEnhanced : BonModule
{
    /// <summary>
    /// Gets the logger for this module.
    /// </summary>
    protected ILogger? Logger { get; private set; }

    /// <summary>
    /// Gets the logger factory for creating loggers.
    /// </summary>
    protected ILoggerFactory? LoggerFactory { get; private set; }

    /// <summary>
    /// Gets the configuration instance.
    /// </summary>
    protected IConfiguration? Configuration { get; private set; }

    /// <summary>
    /// Gets information about this module.
    /// </summary>
    public ModuleInfo? ModuleInfo { get; protected set; }

    /// <summary>
    /// Initializes module properties from the configuration context.
    /// Called automatically during module configuration.
    /// </summary>
    internal void InitializeModuleProperties(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Initialize logger factory and logger
        try
        {
            var tempProvider = context.Services.BuildServiceProvider();
            LoggerFactory = tempProvider.GetService<ILoggerFactory>();
            Logger = LoggerFactory?.CreateLogger(GetType());
            Configuration = tempProvider.GetService<IConfiguration>();
            tempProvider.Dispose();
        }
        catch
        {
            // Logger not available yet - will be null (backward compatible)
        }

        // Initialize module info
        ModuleInfo = GetModuleInfo();
    }

    /// <summary>
    /// Gets module information from attributes or defaults.
    /// Override this method to provide custom module information.
    /// </summary>
    protected virtual ModuleInfo GetModuleInfo()
    {
        var attr = GetType().GetCustomAttribute<ModuleInfoAttribute>();
        return new ModuleInfo
        {
            Name = attr?.Name ?? GetType().Name,
            Version = attr?.Version ?? "1.0.0",
            Description = attr?.Description,
            Author = attr?.Author,
            ModuleType = GetType()
        };
    }

    /// <summary>
    /// Creates a logger for a specific type.
    /// </summary>
    protected ILogger<T> CreateLogger<T>() => LoggerFactory?.CreateLogger<T>() 
        ?? throw new InvalidOperationException("LoggerFactory is not available. Ensure logging is configured.");

    /// <summary>
    /// Configures options from a configuration section in appsettings.json.
    /// </summary>
    protected void ConfigureFromSection<TOptions>(string sectionName) where TOptions : class
    {
        if (Configuration == null)
            throw new InvalidOperationException("Configuration is not available.");

        Services.Configure<TOptions>(Configuration.GetSection(sectionName));
    }

    /// <summary>
    /// Configures options with validation.
    /// </summary>
    protected void ConfigureWithValidation<TOptions>(
        Action<TOptions> configure,
        Func<TOptions, bool> validate)
        where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(configure);
        ArgumentNullException.ThrowIfNull(validate);

        Services.Configure(configure);
        Services.PostConfigure<TOptions>(options =>
        {
            if (!validate(options))
            {
                throw new OptionsValidationException(
                    typeof(TOptions).Name,
                    typeof(TOptions),
                    new[] { "Validation failed for configured options." });
            }
        });
    }

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    protected T? GetConfigValue<T>(string key)
    {
        if (Configuration == null)
            throw new InvalidOperationException("Configuration is not available.");

        return Configuration.GetValue<T>(key);
    }

    /// <summary>
    /// Gets a required configuration value by key.
    /// </summary>
    protected T GetRequiredConfigValue<T>(string key)
    {
        var value = GetConfigValue<T>(key);
        if (value == null)
            throw new InvalidOperationException($"Configuration key '{key}' is required but not found.");

        return value;
    }

    // Health check methods removed - they require ASP.NET Core packages
    // Use health checks in Bonyan.AspNetCore module instead

    /// <summary>
    /// Registers a background service.
    /// </summary>
    protected void AddBackgroundService<T>() where T : class, IHostedService
    {
        Services.AddHostedService<T>();
    }

    /// <summary>
    /// Registers a background service with a factory.
    /// </summary>
    protected void AddBackgroundService(Func<IServiceProvider, IHostedService> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        Services.AddSingleton<IHostedService>(sp => factory(sp));
    }

    /// <summary>
    /// Checks if a module is loaded.
    /// </summary>
    protected bool IsModuleLoaded<TModule>() where TModule : IBonModule
    {
        try
        {
            var tempProvider = Services.BuildServiceProvider();
            var container = tempProvider.GetService<IBonModuleContainer>();
            tempProvider.Dispose();
            return container?.Modules.Any(m => m.ModuleType == typeof(TModule)) ?? false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the module descriptor for a specific module type.
    /// </summary>
    protected BonModuleDescriptor? GetModuleDescriptor<TModule>() where TModule : IBonModule
    {
        try
        {
            var tempProvider = Services.BuildServiceProvider();
            var container = tempProvider.GetService<IBonModuleContainer>();
            tempProvider.Dispose();
            return container?.Modules.FirstOrDefault(m => m.ModuleType == typeof(TModule));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if the current environment is Development.
    /// </summary>
    protected bool IsDevelopment()
    {
        return GetEnvironment() == "Development";
    }

    /// <summary>
    /// Checks if the current environment is Production.
    /// </summary>
    protected bool IsProduction()
    {
        return GetEnvironment() == "Production";
    }

    /// <summary>
    /// Checks if the current environment matches the specified environment.
    /// </summary>
    protected bool IsEnvironment(string environment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environment);
        return string.Equals(GetEnvironment(), environment, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the current environment name.
    /// </summary>
    protected string GetEnvironment()
    {
        if (Configuration == null)
            return "Production";

        return Configuration["ASPNETCORE_ENVIRONMENT"] 
            ?? Configuration["Environment"] 
            ?? "Production";
    }
}

/// <summary>
/// Attribute for providing module metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ModuleInfoAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the module version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the module description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the module author.
    /// </summary>
    public string? Author { get; set; }
}

/// <summary>
/// Represents information about a module.
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the module description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the module author.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the module type.
    /// </summary>
    public Type? ModuleType { get; set; }
}

