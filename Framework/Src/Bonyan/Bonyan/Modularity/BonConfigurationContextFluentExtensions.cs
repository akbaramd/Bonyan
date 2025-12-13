using Bonyan.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity;

/// <summary>
/// Fluent API extensions for <see cref="BonConfigurationContext"/>.
/// Provides a design-system style fluent interface for consistent, discoverable configuration.
/// </summary>
public static class BonConfigurationContextFluentExtensions
{
    /// <summary>
    /// Adds a plugin folder source to the configuration context.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="folderPath">The path to the plugin folder.</param>
    /// <param name="autoDiscoverJson">Whether to automatically discover plugin.json files (default: true).</param>
    /// <param name="searchOption">The search option for scanning subfolders (default: TopDirectoryOnly).</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddPluginFolder(
        this BonConfigurationContext context,
        string folderPath,
        bool autoDiscoverJson = true,
        System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        context.PlugInSources.AddFolder(folderPath, autoDiscoverJson, searchOption);
        
        return context;
    }

    /// <summary>
    /// Adds a JSON plugin manifest source to the configuration context.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="manifestPath">The path to the plugin manifest JSON file.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddPluginManifest(
        this BonConfigurationContext context,
        string manifestPath)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        context.PlugInSources.AddJsonManifest(manifestPath);
        
        return context;
    }

    /// <summary>
    /// Configures options of type <typeparamref name="TOptions"/> and returns the context for chaining.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="configure">The action to configure options.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext WithOptions<TOptions>(
        this BonConfigurationContext context,
        Action<TOptions> configure)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configure);

        context.ConfigureOptions(configure);
        return context;
    }

    /// <summary>
    /// Configures options from a configuration section.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="sectionName">The name of the configuration section.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext WithOptionsFromConfiguration<TOptions>(
        this BonConfigurationContext context,
        string sectionName)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        context.Services.Configure<TOptions>(context.Configuration.GetSection(sectionName));
        return context;
    }

    /// <summary>
    /// Configures and validates options of type <typeparamref name="TOptions"/>.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="configure">The action to configure options.</param>
    /// <param name="validate">The validation function.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext WithValidatedOptions<TOptions>(
        this BonConfigurationContext context,
        Action<TOptions> configure,
        Func<TOptions, bool> validate)
        where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configure);
        ArgumentNullException.ThrowIfNull(validate);

        context.ConfigureAndValidate(configure, validate);
        return context;
    }

    /// <summary>
    /// Adds a service registration to the service collection.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddService<TService, TImplementation>(
        this BonConfigurationContext context,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return context;
    }

    /// <summary>
    /// Adds a singleton service registration.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddSingleton<TService, TImplementation>(
        this BonConfigurationContext context)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Services.AddSingleton<TService, TImplementation>();
        return context;
    }

    /// <summary>
    /// Adds a scoped service registration.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddScoped<TService, TImplementation>(
        this BonConfigurationContext context)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Services.AddScoped<TService, TImplementation>();
        return context;
    }

    /// <summary>
    /// Adds a transient service registration.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddTransient<TService, TImplementation>(
        this BonConfigurationContext context)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Services.AddTransient<TService, TImplementation>();
        return context;
    }

    /// <summary>
    /// Adds a service instance.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="instance">The service instance.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddInstance<TService>(
        this BonConfigurationContext context,
        TService instance)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(instance);

        context.Services.AddSingleton(instance);
        return context;
    }

    /// <summary>
    /// Adds a service factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="factory">The factory function.</param>
    /// <param name="lifetime">The service lifetime (default: Scoped).</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddFactory<TService>(
        this BonConfigurationContext context,
        Func<IServiceProvider, TService> factory,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(factory);

        context.Services.Add(new ServiceDescriptor(typeof(TService), factory, lifetime));
        return context;
    }

    /// <summary>
    /// Configures multiple plugin folders at once.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="folderPaths">The paths to plugin folders.</param>
    /// <param name="autoDiscoverJson">Whether to automatically discover plugin.json files (default: true).</param>
    /// <param name="searchOption">The search option for scanning subfolders (default: TopDirectoryOnly).</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddPluginFolders(
        this BonConfigurationContext context,
        System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly,
        bool autoDiscoverJson = true,
        params string[] folderPaths)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(folderPaths);

        context.PlugInSources.AddFoldersWithJsonDiscovery(folderPaths, searchOption, autoDiscoverJson);

        return context;
    }

    /// <summary>
    /// Configures multiple plugin manifests at once.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="manifestPaths">The paths to plugin manifest files.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext AddPluginManifests(
        this BonConfigurationContext context,
        params string[] manifestPaths)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(manifestPaths);

        context.PlugInSources.AddJsonManifests(manifestPaths);

        return context;
    }

    /// <summary>
    /// Conditionally configures options based on a condition.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="configure">The action to configure options if condition is true.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext WithOptionsIf<TOptions>(
        this BonConfigurationContext context,
        bool condition,
        Action<TOptions> configure)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configure);

        if (condition)
        {
            context.WithOptions(configure);
        }

        return context;
    }

    /// <summary>
    /// Conditionally configures options based on an environment check.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <param name="context">The configuration context.</param>
    /// <param name="environmentName">The environment name to check (e.g., "Development", "Production").</param>
    /// <param name="configure">The action to configure options if environment matches.</param>
    /// <returns>The configuration context for method chaining.</returns>
    public static BonConfigurationContext WithOptionsForEnvironment<TOptions>(
        this BonConfigurationContext context,
        string environmentName,
        Action<TOptions> configure)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);
        ArgumentNullException.ThrowIfNull(configure);

        var currentEnvironment = context.Configuration["ASPNETCORE_ENVIRONMENT"] 
            ?? context.Configuration["Environment"] 
            ?? "Production";

        if (string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase))
        {
            context.WithOptions(configure);
        }

        return context;
    }
}

