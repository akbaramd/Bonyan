using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Centralized logging interface for zone components
/// </summary>
public interface IZoneComponentLogger
{
    void LogError(string componentName, string message, Exception? exception = null);
    void LogWarning(string componentName, string message);
    void LogInformation(string componentName, string message);
}

/// <summary>
/// Default implementation of zone component logger
/// </summary>
public class ZoneComponentLogger : IZoneComponentLogger
{
    private readonly ILogger<ZoneComponentLogger> _logger;

    public ZoneComponentLogger(ILogger<ZoneComponentLogger> logger)
    {
        _logger = logger;
    }

    public void LogError(string componentName, string message, Exception? exception = null)
    {
        _logger.LogError(exception, "Zone Component [{ComponentName}]: {Message}", componentName, message);
    }

    public void LogWarning(string componentName, string message)
    {
        _logger.LogWarning("Zone Component [{ComponentName}]: {Message}", componentName, message);
    }

    public void LogInformation(string componentName, string message)
    {
        _logger.LogInformation("Zone Component [{ComponentName}]: {Message}", componentName, message);
    }
}

/// <summary>
/// Parameter definition for zone components
/// </summary>
public class ZoneComponentParameter
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parameter value
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Parameter type
    /// </summary>
    public Type Type { get; set; } = typeof(object);

    /// <summary>
    /// Whether the parameter is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Default value if not provided
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Parameter description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Validation rules
    /// </summary>
    public List<Func<object?, bool>> Validators { get; set; } = new();

    public ZoneComponentParameter(string name, Type type, bool isRequired = false, object? defaultValue = null, string? description = null)
    {
        Name = name;
        Type = type;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        Description = description;
    }

    /// <summary>
    /// Validate the parameter value
    /// </summary>
    public bool Validate(object? value)
    {
        if (IsRequired && value == null)
            return false;

        if (value != null && !Type.IsAssignableFrom(value.GetType()))
            return false;

        return Validators.All(validator => validator(value));
    }

    /// <summary>
    /// Get typed value
    /// </summary>
    public T? GetValue<T>()
    {
        if (Value == null)
            return default(T);

        try
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }
}

/// <summary>
/// Collection of zone component parameters
/// </summary>
public class ZoneComponentParameters : Dictionary<string, ZoneComponentParameter>
{
    /// <summary>
    /// Add a parameter with validation
    /// </summary>
    public void AddParameter<T>(string name, bool isRequired = false, T? defaultValue = default, string? description = null, params Func<T?, bool>[] validators)
    {
        var parameter = new ZoneComponentParameter(name, typeof(T), isRequired, defaultValue, description);
        
        // Convert validators to object-based validators
        foreach (var validator in validators)
        {
            parameter.Validators.Add(value => validator((T?)value));
        }

        Add(name, parameter);
    }

    /// <summary>
    /// Get a parameter value with type conversion
    /// </summary>
    public T? GetValue<T>(string name, T? defaultValue = default)
    {
        if (TryGetValue(name, out var parameter))
        {
            return parameter.GetValue<T>();
        }
        return defaultValue;
    }

    /// <summary>
    /// Set a parameter value
    /// </summary>
    public void SetValue<T>(string name, T? value)
    {
        if (TryGetValue(name, out var parameter))
        {
            parameter.Value = value;
        }
        else
        {
            AddParameter(name, false, value);
        }
    }

    /// <summary>
    /// Validate all parameters
    /// </summary>
    public bool Validate(out List<string> errors)
    {
        errors = new List<string>();

        foreach (var parameter in Values)
        {
            if (!parameter.Validate(parameter.Value))
            {
                errors.Add($"Parameter '{parameter.Name}' validation failed");
            }
        }

        return errors.Count == 0;
    }
}

/// <summary>
/// Helper class for model mapping with caching and expression-based optimization
/// </summary>
public static class ModelMappingHelper
{
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Action<object, object>>> _mappingCache = new();

    /// <summary>
    /// Convert payload to strongly typed model using cached mapping expressions
    /// </summary>
    public static TModel ConvertPayload<TModel>(object payload, IZoneComponentLogger? logger = null)
    {
        if (payload is TModel model)
            return model;

        try
        {
            var targetModel = Activator.CreateInstance<TModel>();
            if (targetModel == null)
            {
                throw new InvalidOperationException($"Cannot create instance of {typeof(TModel).Name}");
            }

            var mappingDelegate = GetOrCreateMappingDelegate<TModel>(payload.GetType());
            if (mappingDelegate != null)
            {
                mappingDelegate(payload, targetModel);
            }

            return targetModel;
        }
        catch (Exception ex)
        {
            logger?.LogError(typeof(TModel).Name, $"Error converting payload to {typeof(TModel).Name}", ex);
            throw;
        }
    }

    /// <summary>
    /// Get or create a cached mapping delegate for the given source and target types
    /// </summary>
    private static Action<object, object>? GetOrCreateMappingDelegate<TModel>(Type sourceType)
    {
        var targetType = typeof(TModel);
        var typeCache = _mappingCache.GetOrAdd(targetType, _ => new ConcurrentDictionary<Type, Action<object, object>>());
        
        return typeCache.GetOrAdd(sourceType, sourceTypeKey => CreateMappingDelegate<TModel>(sourceTypeKey));
    }

    /// <summary>
    /// Create a compiled expression for mapping from source to target
    /// </summary>
    private static Action<object, object> CreateMappingDelegate<TModel>(Type sourceType)
    {
        var targetType = typeof(TModel);
        var sourceParam = Expression.Parameter(typeof(object), "source");
        var targetParam = Expression.Parameter(typeof(object), "target");

        var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null);

        var mappingExpressions = new List<Expression>();

        // Cast parameters to their actual types
        var typedSource = Expression.Convert(sourceParam, sourceType);
        var typedTarget = Expression.Convert(targetParam, targetType);

        foreach (var targetProperty in targetProperties)
        {
            try
            {
                var sourceProperty = sourceType.GetProperty(targetProperty.Name, 
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                
                if (sourceProperty?.CanRead == true)
                {
                    var sourceValue = Expression.Property(typedSource, sourceProperty);
                    var targetPropertyExpr = Expression.Property(typedTarget, targetProperty);
                    
                    // Add null check and type conversion
                    var convertedValue = Expression.Condition(
                        Expression.Equal(sourceValue, Expression.Constant(null)),
                        Expression.Default(targetProperty.PropertyType),
                        Expression.Convert(sourceValue, targetProperty.PropertyType)
                    );
                    
                    var assignment = Expression.Assign(targetPropertyExpr, convertedValue);
                    mappingExpressions.Add(assignment);
                }
            }
            catch
            {
                // Skip properties that can't be mapped
                continue;
            }
        }

        if (mappingExpressions.Count == 0)
        {
            return (source, target) => { }; // Empty action
        }

        var body = Expression.Block(mappingExpressions);
        var lambda = Expression.Lambda<Action<object, object>>(body, sourceParam, targetParam);
        return lambda.Compile();
    }
}

/// <summary>
/// Custom type converter for complex object mapping
/// </summary>
public class ZoneComponentTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return true; // Allow conversion from any type
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null) return null;
        
        var targetType = context?.PropertyDescriptor?.PropertyType;
        if (targetType == null) return value;

        try
        {
            return Convert.ChangeType(value, targetType, culture);
        }
        catch
        {
            // Return original value if conversion fails
            return value;
        }
    }
}

/// <summary>
/// Base class for zone components with strongly typed input models and advanced parameters
/// </summary>
/// <typeparam name="TModel">The type of the input model that must implement IZone</typeparam>
public abstract class ZoneViewComponent<TModel> : IZoneComponent where TModel : IZone
{
    private static readonly string _componentId = $"{typeof(ZoneViewComponent<TModel>).AssemblyQualifiedName}";

    /// <summary>
    /// Unique identifier for this component instance using assembly qualified name
    /// </summary>
    public virtual string Id => _componentId;

    /// <summary>
    /// Gets the zones this component should be rendered in
    /// Automatically gets zone name from IZone interface if TModel implements IZone
    /// </summary>
    public virtual IEnumerable<string> Zones
    {
        get
        {
            // Check if TModel implements IZone
            if (typeof(IZone).IsAssignableFrom(typeof(TModel)))
            {
                // Return the zone name from the IZone interface
                return new[] { GetZoneNameFromIZone() };
            }
            
            // Fallback to abstract implementation for backward compatibility
            return GetZonesFromImplementation();
        }
    }

    /// <summary>
    /// Abstract method for getting zones (for backward compatibility)
    /// Override this only if TModel doesn't implement IZone
    /// </summary>
    protected virtual IEnumerable<string> GetZonesFromImplementation()
    {
        throw new NotImplementedException($"Either implement IZone on {typeof(TModel).Name} or override GetZonesFromImplementation()");
    }

    /// <summary>
    /// Gets the zone name from IZone interface
    /// </summary>
    private string GetZoneNameFromIZone()
    {
        // Create a temporary instance to get the zone name
        try
        {
            var instance = Activator.CreateInstance<TModel>();
            if (instance is IZone zone)
            {
                return zone.ZoneName;
            }
        }
        catch
        {
            // If we can't create an instance, try to get it from the type itself
            // This is a fallback for cases where the model requires parameters
        }

        // Fallback: try to get zone name from static property or attribute
        var zoneNameProperty = typeof(TModel).GetProperty("ZoneName", BindingFlags.Public | BindingFlags.Static);
        if (zoneNameProperty?.CanRead == true)
        {
            var zoneName = zoneNameProperty.GetValue(null) as string;
            if (!string.IsNullOrEmpty(zoneName))
            {
                return zoneName;
            }
        }

        // Last resort: use the type name as zone name
        return typeof(TModel).Name.Replace("Zone", "").ToLowerInvariant();
    }

    /// <summary>
    /// Gets the priority/order for rendering within zones (lower numbers render first)
    /// </summary>
    public abstract int Priority { get; }

    /// <summary>
    /// Gets the component parameters definition
    /// Override this to define parameters for your component
    /// </summary>
    public virtual ZoneComponentParameters Parameters => new();

    /// <summary>
    /// Initialize parameters with default values
    /// Override this to set up default parameters
    /// </summary>
    protected virtual void InitializeParameters()
    {
        // Override in derived classes to add parameters
    }

    /// <summary>
    /// Handle the zone component with strongly typed model and parameters
    /// </summary>
    /// <param name="model">The strongly typed input model</param>
    /// <param name="context">The zone rendering context</param>
    /// <param name="parameters">The component parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Zone component result</returns>
    public abstract Task<ZoneComponentResult> HandleAsync(TModel model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// IZoneComponent implementation - converts payload and calls strongly typed version
    /// </summary>
    public async Task<ZoneComponentResult> HandleAsync(object payload, ZoneRenderingContext context)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        
        try
        {
            var model = ModelMappingHelper.ConvertPayload<TModel>(payload, logger);
            
            // Initialize and validate parameters
            var parameters = Parameters;
            InitializeParameters();
            
            // Extract parameters from payload if it's a dictionary
            if (payload is IDictionary<string, object> payloadDict)
            {
                foreach (var kvp in payloadDict)
                {
                    if (parameters.TryGetValue(kvp.Key, out var parameter))
                    {
                        parameter.Value = kvp.Value;
                    }
                }
            }

            // Validate parameters
            if (!parameters.Validate(out var errors))
            {
                logger?.LogError(Id, $"Parameter validation failed: {string.Join(", ", errors)}");
                throw new ArgumentException($"Parameter validation failed: {string.Join(", ", errors)}");
            }

            return await HandleAsync(model, context, parameters, cancellationToken: default);
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, $"Error handling component: {ex.Message}", ex);
            throw; // Re-throw to allow proper error handling upstream
        }
    }

    /// <summary>
    /// Convert payload to strongly typed model (legacy method for backward compatibility)
    /// </summary>
    /// <param name="payload">The payload object</param>
    /// <returns>The converted model</returns>
    [Obsolete("Use ModelMappingHelper.ConvertPayload instead")]
    protected virtual TModel ConvertPayload(object payload)
    {
        return ModelMappingHelper.ConvertPayload<TModel>(payload);
    }
}

/// <summary>
/// Base class for zone components with strongly typed input models, context, and advanced parameters
/// </summary>
/// <typeparam name="TModel">The type of the input model</typeparam>
/// <typeparam name="TContext">The type of the additional context</typeparam>
///

public abstract class ZoneViewComponent<TModel, TContext> : IZoneComponent
{
    private static readonly string _componentId = $"{typeof(ZoneViewComponent<TModel, TContext>).AssemblyQualifiedName}";

    /// <summary>
    /// Unique identifier for this component instance using assembly qualified name
    /// </summary>
    public virtual string Id => _componentId;

    /// <summary>
    /// Gets the zones this component should be rendered in
    /// Automatically gets zone name from IZone interface if TModel implements IZone
    /// </summary>
    public virtual IEnumerable<string> Zones
    {
        get
        {
            // Check if TModel implements IZone
            if (typeof(IZone).IsAssignableFrom(typeof(TModel)))
            {
                // Return the zone name from the IZone interface
                return new[] { GetZoneNameFromIZone() };
            }
            
            // Fallback to abstract implementation for backward compatibility
            return GetZonesFromImplementation();
        }
    }

    /// <summary>
    /// Abstract method for getting zones (for backward compatibility)
    /// Override this only if TModel doesn't implement IZone
    /// </summary>
    protected virtual IEnumerable<string> GetZonesFromImplementation()
    {
        throw new NotImplementedException($"Either implement IZone on {typeof(TModel).Name} or override GetZonesFromImplementation()");
    }

    /// <summary>
    /// Gets the zone name from IZone interface
    /// </summary>
    private string GetZoneNameFromIZone()
    {
        // Create a temporary instance to get the zone name
        try
        {
            var instance = Activator.CreateInstance<TModel>();
            if (instance is IZone zone)
            {
                return zone.ZoneName;
            }
        }
        catch
        {
            // If we can't create an instance, try to get it from the type itself
            // This is a fallback for cases where the model requires parameters
        }

        // Fallback: try to get zone name from static property or attribute
        var zoneNameProperty = typeof(TModel).GetProperty("ZoneName", BindingFlags.Public | BindingFlags.Static);
        if (zoneNameProperty?.CanRead == true)
        {
            var zoneName = zoneNameProperty.GetValue(null) as string;
            if (!string.IsNullOrEmpty(zoneName))
            {
                return zoneName;
            }
        }

        // Last resort: use the type name as zone name
        return typeof(TModel).Name.Replace("Zone", "").ToLowerInvariant();
    }

    /// <summary>
    /// Gets the priority/order for rendering within zones (lower numbers render first)
    /// </summary>
    public abstract int Priority { get; }

    /// <summary>
    /// Gets the component parameters definition
    /// Override this to define parameters for your component
    /// </summary>
    public virtual ZoneComponentParameters Parameters => new();

    /// <summary>
    /// Initialize parameters with default values
    /// Override this to set up default parameters
    /// </summary>
    protected virtual void InitializeParameters()
    {
        // Override in derived classes to add parameters
    }

    /// <summary>
    /// Handle the zone component with strongly typed model, context, and parameters
    /// </summary>
    /// <param name="model">The strongly typed input model</param>
    /// <param name="context">The zone rendering context</param>
    /// <param name="componentContext">The additional component context</param>
    /// <param name="parameters">The component parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Zone component result</returns>
    public abstract Task<ZoneComponentResult> HandleAsync(TModel model, ZoneRenderingContext context, TContext componentContext, ZoneComponentParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// IZoneComponent implementation - extracts model and context, then calls strongly typed version
    /// </summary>
    public async Task<ZoneComponentResult> HandleAsync(object payload, ZoneRenderingContext context)
    {
        var logger = context.GetServiceOrDefault<IZoneComponentLogger>();
        
        try
        {
            var (model, componentContext) = ExtractModelAndContext(payload, logger);
            
            // Initialize and validate parameters
            var parameters = Parameters;
            InitializeParameters();
            
            // Extract parameters from payload if it's a dictionary
            if (payload is IDictionary<string, object> payloadDict)
            {
                foreach (var kvp in payloadDict)
                {
                    if (parameters.TryGetValue(kvp.Key, out var parameter))
                    {
                        parameter.Value = kvp.Value;
                    }
                }
            }

            // Validate parameters
            if (!parameters.Validate(out var errors))
            {
                logger?.LogError(Id, $"Parameter validation failed: {string.Join(", ", errors)}");
                throw new ArgumentException($"Parameter validation failed: {string.Join(", ", errors)}");
            }

            return await HandleAsync(model, context, componentContext, parameters, cancellationToken: default);
        }
        catch (Exception ex)
        {
            logger?.LogError(Id, $"Error handling component: {ex.Message}", ex);
            throw; // Re-throw to allow proper error handling upstream
        }
    }

    /// <summary>
    /// Extract model and component context from payload
    /// </summary>
    /// <param name="payload">The payload object</param>
    /// <param name="logger">Optional logger for error reporting</param>
    /// <returns>Tuple containing model and component context</returns>
    protected virtual (TModel model, TContext componentContext) ExtractModelAndContext(object payload, IZoneComponentLogger? logger = null)
    {
        var model = ModelMappingHelper.ConvertPayload<TModel>(payload, logger);
        var componentContext = CreateDefaultContext();
        return (model, componentContext);
    }

    /// <summary>
    /// Create default component context when context is not provided
    /// </summary>
    /// <returns>Default context instance</returns>
    protected virtual TContext CreateDefaultContext()
    {
        if (typeof(TContext).IsValueType)
        {
            return default(TContext)!;
        }
        
        try
        {
            return Activator.CreateInstance<TContext>();
        }
        catch
        {
            throw new InvalidOperationException($"Cannot create default context for type {typeof(TContext).Name}");
        }
    }
}

/// <summary>
/// Result of a zone component operation with enhanced parameters
/// </summary>
public class ZoneComponentResult
{
    /// <summary>
    /// The HTML content to append
    /// </summary>
    public IHtmlContent? HtmlContent { get; set; }

    /// <summary>
    /// The view name to render (if not using HtmlContent)
    /// </summary>
    public string? ViewName { get; set; }

    /// <summary>
    /// The model to pass to the view
    /// </summary>
    public object? ViewModel { get; set; }

    /// <summary>
    /// Whether to continue processing other components in the zone
    /// </summary>
    public bool ContinueProcessing { get; set; } = true;

    /// <summary>
    /// Parameters that can be passed to child components or used for rendering
    /// </summary>
    public ZoneComponentParameters? Parameters { get; set; }

    /// <summary>
    /// Metadata that can be used during rendering (for backward compatibility)
    /// </summary>
    public IDictionary<string, object>? Meta { get; set; }

    /// <summary>
    /// Create a result with HTML content
    /// </summary>
    public static ZoneComponentResult Html(IHtmlContent htmlContent, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new ZoneComponentResult
        {
            HtmlContent = htmlContent,
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta
        };
    }

    /// <summary>
    /// Create a result with HTML string
    /// </summary>
    public static ZoneComponentResult Html(string html, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new ZoneComponentResult
        {
            HtmlContent = new HtmlString(html),
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta
        };
    }

    /// <summary>
    /// Create a result with a view to render
    /// </summary>
    public static ZoneComponentResult View(string viewName, object? viewModel = null, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new ZoneComponentResult
        {
            ViewName = viewName,
            ViewModel = viewModel,
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta
        };
    }

    /// <summary>
    /// Create a result that stops processing
    /// </summary>
    public static ZoneComponentResult Stop(ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new ZoneComponentResult
        {
            ContinueProcessing = false,
            Parameters = parameters,
            Meta = meta
        };
    }
}

/// <summary>
/// Context for zone rendering operations with thread-safe collections, view caching, and parameter support
/// </summary>
public class ZoneRenderingContext : IDisposable
{
    private static readonly string[] _allowedViewPaths = { "ZoneViews","Pages","Views", "Components", "Shared" };
    
    // Track if this context has been disposed
    private bool _disposed = false;
    private readonly object _disposeLock = new object();

    /// <summary>
    /// The current ViewContext
    /// </summary>
    public ViewContext ViewContext { get; }

    /// <summary>
    /// The service provider
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// The zone being rendered
    /// </summary>
    public string Zone { get; }

    /// <summary>
    /// The original payload passed to the zone
    /// </summary>
    public object? Payload { get; }

    /// <summary>
    /// Additional data that can be shared between components (thread-safe)
    /// </summary>
    public ConcurrentDictionary<string, object> SharedData { get; }

    /// <summary>
    /// Parameters that can be shared between components (thread-safe)
    /// </summary>
    public ZoneComponentParameters SharedParameters { get; }

    /// <summary>
    /// Metadata collected from all components in this zone (thread-safe)
    /// </summary>
    public ConcurrentDictionary<string, object> ZoneMeta { get; }

    /// <summary>
    /// The scoped service provider for this context
    /// </summary>
    private IServiceProvider? _scopedServiceProvider;

    public ZoneRenderingContext(ViewContext viewContext, IServiceProvider serviceProvider, string zone, object? payload)
    {
        ViewContext = viewContext ?? throw new ArgumentNullException(nameof(viewContext));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Zone = zone ?? throw new ArgumentNullException(nameof(zone));
        Payload = payload;
        SharedData = new ConcurrentDictionary<string, object>();
        SharedParameters = new ZoneComponentParameters();
        ZoneMeta = new ConcurrentDictionary<string, object>();
        
        // Initialize the scoped service provider
        _scopedServiceProvider = GetScopedServiceProvider();
    }

    /// <summary>
    /// Get a service from the service provider
    /// </summary>
    public T GetService<T>() where T : class
    {
        ThrowIfDisposed();
        return GetRequiredScopedService<T>();
    }

    /// <summary>
    /// Try to get a service from the service provider
    /// </summary>
    public T? GetServiceOrDefault<T>() where T : class
    {
        ThrowIfDisposed();
        return GetScopedService<T>();
    }

    /// <summary>
    /// Add metadata to the zone context (thread-safe)
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <param name="value">The metadata value</param>
    public void AddMeta(string key, object value)
    {
        ThrowIfDisposed();
        ZoneMeta.AddOrUpdate(key, value, (_, _) => value);
    }

    /// <summary>
    /// Get metadata from the zone context (thread-safe)
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <returns>The metadata value or null if not found</returns>
    public object? GetMeta(string key)
    {
        ThrowIfDisposed();
        return ZoneMeta.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Get metadata from the zone context with type conversion (thread-safe)
    /// </summary>
    /// <typeparam name="T">The expected type</typeparam>
    /// <param name="key">The metadata key</param>
    /// <returns>The metadata value converted to the specified type or default</returns>
    public T? GetMeta<T>(string key)
    {
        ThrowIfDisposed();
        var value = GetMeta(key);
        if (value is T typedValue)
            return typedValue;
        
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// Add a shared parameter
    /// </summary>
    public void AddParameter<T>(string name, T? value, bool isRequired = false, T? defaultValue = default, string? description = null)
    {
        ThrowIfDisposed();
        SharedParameters.AddParameter(name, isRequired, defaultValue, description);
        SharedParameters.SetValue(name, value);
    }

    /// <summary>
    /// Get a shared parameter value
    /// </summary>
    public T? GetParameter<T>(string name, T? defaultValue = default)
    {
        ThrowIfDisposed();
        return SharedParameters.GetValue(name, defaultValue);
    }

    /// <summary>
    /// Validate view name to ensure it's within allowed paths
    /// </summary>
    /// <param name="viewName">The view name to validate</param>
    /// <returns>True if the view name is valid</returns>
    private bool IsValidViewName(string viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return false;

        // Check if view name contains any path traversal attempts
        if (viewName.Contains("..") || viewName.Contains("\\") || viewName.StartsWith("/"))
            return false;

        // Check if view name starts with allowed paths
        return _allowedViewPaths.Any(path => viewName.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Render a partial view with validation using the same service provider scope
    /// </summary>
    public async Task<string> RenderPartialViewAsync(string viewName, object? model = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var logger = GetServiceOrDefault<IZoneComponentLogger>();
        
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if the service provider is still valid
            if (!IsServiceProviderValid())
            {
                logger?.LogError("ZoneRenderingContext", $"Service provider is disposed for view '{viewName}'");
                return $"<div class='alert alert-danger'>Service provider is disposed. Cannot render view '{viewName}'.</div>";
            }

            if (!IsValidViewName(viewName))
            {
                var errorMessage = $"Invalid view name '{viewName}'. View names must be within allowed paths: {string.Join(", ", _allowedViewPaths)}";
                logger?.LogError("ZoneRenderingContext", errorMessage);
                throw new ArgumentException(errorMessage, nameof(viewName));
            }

            // Get view engine from the same service provider scope as the main UI
            var viewEngine = GetRequiredScopedService<ICompositeViewEngine>();
            
            // Find view directly without caching
            var viewResult = FindViewWithFallback(viewEngine, viewName);

            if (!viewResult.Success)
            {
                var errorMessage = $"View '{viewName}' not found. Searched locations: {string.Join(", ", viewResult.SearchedLocations)}";
                logger?.LogError("ZoneRenderingContext", errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Additional check to ensure the view is not disposed
            try
            {
                var _ = viewResult.View;
            }
            catch (ObjectDisposedException)
            {
                throw new InvalidOperationException($"View '{viewName}' is disposed.");
            }

            // Create a temporary writer to capture the output
            using var stringWriter = new StringWriter();
            
            // Get the model metadata provider from the service provider
            var modelMetadataProvider = GetScopedService<IModelMetadataProvider>() ?? new EmptyModelMetadataProvider();
            
            // Create a new view context that COMPLETELY reuses the original context's infrastructure
            // This ensures we're using the same service provider scope and prevents disposal issues
            var viewContext2 = new ViewContext(
                ViewContext,  // Use the original context as the base
                viewResult.View,
                new ViewDataDictionary<object>(modelMetadataProvider, ViewContext.ViewData.ModelState) { Model = model },
                stringWriter);

            // Note: View buffer scope is automatically handled by ASP.NET Core
            // The ViewContext constructor will handle the buffer scope internally
            // We don't need to manually manage it as it's part of the MVC infrastructure

            // Render the view with comprehensive error handling
            try
            {
                await viewResult.View.RenderAsync(viewContext2);
                return stringWriter.ToString();
            }
            catch (ObjectDisposedException ex)
            {
                logger?.LogError("ZoneRenderingContext", $"ObjectDisposedException while rendering view '{viewName}': {ex.Message}", ex);
                return $"<div class='alert alert-warning'>View temporarily unavailable. Please refresh the page.</div>";
            }
            catch (InvalidOperationException ex)
            {
                logger?.LogError("ZoneRenderingContext", $"InvalidOperationException while rendering view '{viewName}': {ex.Message}", ex);
                return $"<div class='alert alert-danger'>Invalid operation while rendering view: {ex.Message}</div>";
            }
            catch (Exception ex)
            {
                logger?.LogError("ZoneRenderingContext", $"Unexpected error while rendering view '{viewName}': {ex.Message}", ex);
                return $"<div class='alert alert-danger'>Unexpected error while rendering view: {ex.Message}</div>";
            }
        }
        catch (OperationCanceledException)
        {
            logger?.LogInformation("ZoneRenderingContext", $"View rendering cancelled for '{viewName}'");
            throw;
        }
        catch (ArgumentException ex)
        {
            logger?.LogError("ZoneRenderingContext", $"Invalid argument for view '{viewName}': {ex.Message}", ex);
            return $"<div class='alert alert-danger'>Invalid view name: {ex.Message}</div>";
        }
        catch (InvalidOperationException ex)
        {
            logger?.LogError("ZoneRenderingContext", $"View not found '{viewName}': {ex.Message}", ex);
            return $"<div class='alert alert-danger'>View not found: {ex.Message}</div>";
        }
        catch (Exception ex)
        {
            logger?.LogError("ZoneRenderingContext", $"Critical error rendering view '{viewName}': {ex.Message}", ex);
            return $"<div class='alert alert-danger'>Critical error rendering view: {ex.Message}</div>";
        }
    }

    /// <summary>
    /// Find view with fallback strategies
    /// </summary>
    private ViewEngineResult FindViewWithFallback(ICompositeViewEngine viewEngine, string viewName)
    {
        try
        {
            var viewResult = viewEngine.FindView(ViewContext, viewName, false);
            
            if (!viewResult.Success)
            {
                // Try alternative view name patterns
                var alternativeNames = new[]
                {
                    viewName.Replace("Partials/", ""),
                    viewName.Replace("_", ""),
                    viewName + ".cshtml",
                    "Partials/" + viewName,
                    "_" + viewName
                };
                
                foreach (var altName in alternativeNames)
                {
                    viewResult = viewEngine.FindView(ViewContext, altName, false);
                    if (viewResult.Success)
                        break;
                }
            }

            return viewResult;
        }
        catch (ObjectDisposedException)
        {
            // If the view engine is disposed, return a failed result
            return ViewEngineResult.NotFound(viewName, new[] { viewName });
        }
        catch (Exception ex)
        {
            // For any other exception, return a failed result
            return ViewEngineResult.NotFound(viewName, new[] { viewName });
        }
    }

    /// <summary>
    /// Render a partial view as HTML content
    /// </summary>
    public async Task<IHtmlContent> RenderPartialViewAsHtmlAsync(string viewName, object? model = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var html = await RenderPartialViewAsync(viewName, model, cancellationToken);
        return new HtmlString(html);
    }

    /// <summary>
    /// Clear the view cache (useful for development scenarios)
    /// </summary>
    public static void ClearViewCache()
    {
        // No-op since we removed caching
    }

    /// <summary>
    /// Ensure we're using the same service provider scope as the main UI
    /// </summary>
    private IServiceProvider GetScopedServiceProvider()
    {
        try
        {
            // Try to get the scoped service provider from the HTTP context
            var httpContext = ViewContext.HttpContext;
            if (httpContext != null && !httpContext.RequestAborted.IsCancellationRequested)
            {
                // Use the same service provider as the HTTP context
                return httpContext.RequestServices;
            }
        }
        catch (ObjectDisposedException)
        {
            // HTTP context is disposed, fall back to original provider
        }
        catch (Exception)
        {
            // Any other exception, fall back to original provider
        }
        
        // Fallback to the original service provider
        return ServiceProvider;
    }

    /// <summary>
    /// Safely get a service from the scoped service provider
    /// </summary>
    private T? GetScopedService<T>() where T : class
    {
        try
        {
            // Use cached scoped provider if available
            var scopedProvider = _scopedServiceProvider ?? GetScopedServiceProvider();
            if (scopedProvider == null)
                return null;

            return scopedProvider.GetService<T>();
        }
        catch (ObjectDisposedException)
        {
            // If the service provider is disposed, try to refresh it
            try
            {
                _scopedServiceProvider = GetScopedServiceProvider();
                return _scopedServiceProvider?.GetService<T>();
            }
            catch
            {
                return null;
            }
        }
        catch (Exception)
        {
            // For any other exception, return null
            return null;
        }
    }

    /// <summary>
    /// Safely get a required service from the scoped service provider
    /// </summary>
    private T GetRequiredScopedService<T>() where T : class
    {
        var service = GetScopedService<T>();
        if (service == null)
        {
            throw new InvalidOperationException($"Required service of type {typeof(T).Name} is not available in the current scope. Zone: {Zone}");
        }
        return service;
    }

    /// <summary>
    /// Check if the service provider is still valid and not disposed
    /// </summary>
    private bool IsServiceProviderValid()
    {
        try
        {
            // Check if this context is disposed
            if (_disposed)
                return false;

            var httpContext = ViewContext.HttpContext;
            if (httpContext != null)
            {
                // Check if HTTP context is disposed
                if (httpContext.RequestAborted.IsCancellationRequested)
                    return false;

                // Try to access the request services to see if they're still valid
                var _ = httpContext.RequestServices;
                return true;
            }
            
            // Check if the original service provider is still valid
            return ServiceProvider != null;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Throw if this context has been disposed
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ZoneRenderingContext), $"ZoneRenderingContext for zone '{Zone}' has been disposed.");
        }
    }

    /// <summary>
    /// Dispose this context and clean up resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose pattern implementation
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        lock (_disposeLock)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Clear cached data
                SharedData?.Clear();
                SharedParameters?.Clear();
                ZoneMeta?.Clear();
                
                // Clear scoped service provider reference
                _scopedServiceProvider = null;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~ZoneRenderingContext()
    {
        Dispose(false);
    }
} 