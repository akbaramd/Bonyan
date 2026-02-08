# Fluent API Design Proposal for Bonyan Modularity

## 🎯 Goals

1. **Fluent API** for easy module configuration
2. **Configuration Property** inside modules for type-safe access
3. **Behavior Methods** for configuration (get/set/validate)
4. **Easy Module Setup** with builder pattern

---

## 📋 Questions to Clarify Requirements

### 1. Configuration Scope
- **Q1**: Should each module have its own isolated configuration, or shared configuration?
- **Q2**: Do you want module-specific configuration classes (e.g., `MyModuleOptions`) or a single `ModuleConfiguration`?
- **Q3**: Should configuration be accessible at both module class level AND context level?

### 2. Fluent API Style
- **Q4**: Do you prefer method chaining (`.Configure().With().Enable()`) or property-based fluent API (`.Configuration.Set().Enable()`)?
- **Q5**: Should fluent API work in both `OnConfigureAsync` context AND module constructor/initialization?

### 3. Configuration Access
- **Q6**: Should configuration be:
  - **Immutable** after module configuration phase?
  - **Mutable** throughout module lifecycle?
  - **Read-only** after initialization?

### 4. Validation & Defaults
- **Q7**: Should configuration have:
  - Built-in validation (required fields, ranges, etc.)?
  - Default values?
  - Validation callbacks?

### 5. Module-Specific Configuration
- **Q8**: Should each module type have a strongly-typed configuration class?
- **Q9**: Should configuration be discoverable/queryable across modules?

---

## 💡 Proposed Fluent API Design

### Option A: Method Chaining Fluent API

```csharp
// In module's OnConfigureAsync
public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
{
    context
        .ConfigureModule<MyModuleOptions>()
        .With(options => {
            options.EnableFeature = true;
            options.Timeout = TimeSpan.FromSeconds(30);
        })
        .Validate(options => options.Timeout > TimeSpan.Zero)
        .Register();
    
    return ValueTask.CompletedTask;
}
```

### Option B: Module-Level Configuration Property

```csharp
public class MyModule : BonModule
{
    // Configuration property accessible throughout module
    public MyModuleOptions Configuration { get; private set; }
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        // Set configuration via fluent API
        Configuration = context
            .ConfigureModule<MyModuleOptions>()
            .With(options => {
                options.EnableFeature = true;
                options.Timeout = TimeSpan.FromSeconds(30);
            })
            .Validate(options => options.Timeout > TimeSpan.Zero)
            .Build();
        
        // Use configuration later
        if (Configuration.EnableFeature)
        {
            // ...
        }
        
        return ValueTask.CompletedTask;
    }
}
```

### Option C: Builder Pattern with Module Configuration

```csharp
public class MyModule : BonModule
{
    public MyModuleConfiguration Configuration { get; private set; }
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Configuration = new MyModuleConfigurationBuilder(context)
            .EnableFeature(true)
            .SetTimeout(TimeSpan.FromSeconds(30))
            .WithValidation(options => options.Timeout > TimeSpan.Zero)
            .Build();
        
        return ValueTask.CompletedTask;
    }
}
```

---

## 🏗️ Recommended Approach: Hybrid Design

### 1. Fluent Extension Methods on Context

```csharp
// Extension methods for BonConfigurationContext
public static class BonConfigurationContextExtensions
{
    public static IModuleConfigurationBuilder<TOptions> ConfigureModule<TOptions>(
        this BonConfigurationContext context) 
        where TOptions : class, new()
    {
        return new ModuleConfigurationBuilder<TOptions>(context);
    }
    
    public static IModuleConfigurationBuilder<TOptions> ConfigureModule<TOptions>(
        this BonConfigurationContext context,
        Action<TOptions> configureAction) 
        where TOptions : class, new()
    {
        return new ModuleConfigurationBuilder<TOptions>(context)
            .With(configureAction);
    }
}
```

### 2. Module Configuration Builder

```csharp
public interface IModuleConfigurationBuilder<TOptions> where TOptions : class
{
    IModuleConfigurationBuilder<TOptions> With(Action<TOptions> configure);
    IModuleConfigurationBuilder<TOptions> WithDefault();
    IModuleConfigurationBuilder<TOptions> FromConfiguration(string section);
    IModuleConfigurationBuilder<TOptions> Validate(Func<TOptions, bool> validator);
    IModuleConfigurationBuilder<TOptions> ValidateWith<TValidator>() where TValidator : IOptionsValidator<TOptions>;
    IModuleConfigurationBuilder<TOptions> Named(string name);
    TOptions Build();
    TOptions Register();
}

public class ModuleConfigurationBuilder<TOptions> : IModuleConfigurationBuilder<TOptions> 
    where TOptions : class, new()
{
    private readonly BonConfigurationContext _context;
    private readonly TOptions _options;
    private readonly List<Func<TOptions, bool>> _validators = new();
    private string? _namedOptions;
    
    public ModuleConfigurationBuilder(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        _context = context;
        _options = new TOptions();
    }
    
    public IModuleConfigurationBuilder<TOptions> With(Action<TOptions> configure)
    {
        configure(_options);
        return this;
    }
    
    public IModuleConfigurationBuilder<TOptions> WithDefault()
    {
        // Apply default values
        return this;
    }
    
    public IModuleConfigurationBuilder<TOptions> FromConfiguration(string section)
    {
        _context.Configuration.GetSection(section).Bind(_options);
        return this;
    }
    
    public IModuleConfigurationBuilder<TOptions> Validate(Func<TOptions, bool> validator)
    {
        _validators.Add(validator);
        return this;
    }
    
    public IModuleConfigurationBuilder<TOptions> ValidateWith<TValidator>() 
        where TValidator : IOptionsValidator<TOptions>
    {
        // Register validator
        return this;
    }
    
    public IModuleConfigurationBuilder<TOptions> Named(string name)
    {
        _namedOptions = name;
        return this;
    }
    
    public TOptions Build()
    {
        // Validate
        foreach (var validator in _validators)
        {
            if (!validator(_options))
            {
                throw new OptionsValidationException(
                    typeof(TOptions).Name, 
                    typeof(TOptions), 
                    new[] { "Configuration validation failed." });
            }
        }
        
        return _options;
    }
    
    public TOptions Register()
    {
        var options = Build();
        
        if (_namedOptions != null)
        {
            _context.Services.Configure(_namedOptions, _ => options);
        }
        else
        {
            _context.Services.Configure<TOptions>(_ => options);
        }
        
        return options;
    }
}
```

### 3. Module Configuration Property Helper

```csharp
public abstract class BonModule
{
    // ... existing code ...
    
    /// <summary>
    /// Gets or sets the module's configuration.
    /// This property is set during OnConfigureAsync and is available throughout the module lifecycle.
    /// </summary>
    protected TConfiguration? GetConfiguration<TConfiguration>() 
        where TConfiguration : class
    {
        return Services?.GetService<IOptions<TConfiguration>>()?.Value;
    }
    
    /// <summary>
    /// Gets the required configuration for this module.
    /// </summary>
    protected TConfiguration GetRequiredConfiguration<TConfiguration>() 
        where TConfiguration : class
    {
        return Services?.GetRequiredService<IOptions<TConfiguration>>()?.Value
            ?? throw new InvalidOperationException(
                $"Configuration {typeof(TConfiguration).Name} is not registered. " +
                "Ensure it is configured in OnConfigureAsync.");
    }
}
```

### 4. Module-Specific Configuration Pattern

```csharp
// Module with strongly-typed configuration
public class NotificationModule : BonModule
{
    private NotificationModuleOptions? _configuration;
    
    /// <summary>
    /// Gets the module's configuration.
    /// Available after OnConfigureAsync completes.
    /// </summary>
    public NotificationModuleOptions Configuration => 
        _configuration ?? throw new InvalidOperationException(
            "Configuration is not available. Ensure OnConfigureAsync has completed.");
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        // Fluent API configuration
        _configuration = context
            .ConfigureModule<NotificationModuleOptions>()
            .FromConfiguration("Notification")
            .With(options => {
                options.EnableEmail = true;
                options.EnableSms = false;
                options.RetryCount = 3;
            })
            .Validate(options => options.RetryCount > 0)
            .Register();
        
        // Register services based on configuration
        if (_configuration.EnableEmail)
        {
            context.Services.AddScoped<IEmailService, EmailService>();
        }
        
        return ValueTask.CompletedTask;
    }
    
    public override ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken ct)
    {
        // Use configuration during initialization
        if (Configuration.EnableSms)
        {
            // Initialize SMS service
        }
        
        return ValueTask.CompletedTask;
    }
}
```

---

## 🎨 Usage Examples

### Example 1: Simple Configuration

```csharp
public class IdentityModule : BonModule
{
    public IdentityModuleOptions Configuration { get; private set; }
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Configuration = context
            .ConfigureModule<IdentityModuleOptions>()
            .With(options => {
                options.RequireEmailVerification = true;
                options.PasswordMinLength = 8;
            })
            .Register();
        
        return ValueTask.CompletedTask;
    }
}
```

### Example 2: Configuration from appsettings.json

```csharp
public class DatabaseModule : BonModule
{
    public DatabaseModuleOptions Configuration { get; private set; }
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Configuration = context
            .ConfigureModule<DatabaseModuleOptions>()
            .FromConfiguration("Database")
            .With(options => {
                // Override or supplement from appsettings
                options.EnableRetry = true;
            })
            .Validate(options => !string.IsNullOrEmpty(options.ConnectionString))
            .Register();
        
        return ValueTask.CompletedTask;
    }
}
```

### Example 3: Named Configuration

```csharp
public class MultiTenantModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        // Configure multiple named instances
        context
            .ConfigureModule<TenantOptions>()
            .Named("Primary")
            .With(options => options.Name = "Primary Tenant")
            .Register();
        
        context
            .ConfigureModule<TenantOptions>()
            .Named("Secondary")
            .With(options => options.Name = "Secondary Tenant")
            .Register();
        
        return ValueTask.CompletedTask;
    }
}
```

### Example 4: Complex Validation

```csharp
public class ApiModule : BonModule
{
    public ApiModuleOptions Configuration { get; private set; }
    
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Configuration = context
            .ConfigureModule<ApiModuleOptions>()
            .FromConfiguration("Api")
            .With(options => {
                options.Timeout = TimeSpan.FromSeconds(30);
            })
            .Validate(options => options.Timeout > TimeSpan.Zero)
            .Validate(options => options.RateLimit > 0)
            .ValidateWith<ApiOptionsValidator>() // Custom validator
            .Register();
        
        return ValueTask.CompletedTask;
    }
}
```

### Example 5: Fluent Service Registration

```csharp
public class CachingModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        var config = context
            .ConfigureModule<CacheOptions>()
            .With(options => {
                options.Provider = CacheProvider.Redis;
                options.DefaultTtl = TimeSpan.FromHours(1);
            })
            .Register();
        
        // Fluent service registration based on config
        context.Services
            .AddCache(config.Provider)
            .WithTtl(config.DefaultTtl)
            .WithDistributed(config.EnableDistributed);
        
        return ValueTask.CompletedTask;
    }
}
```

---

## 🔧 Additional Fluent API Ideas

### 1. Module Dependency Fluent API

```csharp
public class MyModule : BonModule
{
    public MyModule()
    {
        // Fluent dependency declaration
        this
            .DependsOn<CoreModule>()
            .DependsOn<IdentityModule>()
            .DependsOn<DatabaseModule>();
    }
}
```

### 2. Service Registration Fluent API

```csharp
public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
{
    context.Services
        .AddScoped<IMyService, MyService>()
        .WithLifetime(ServiceLifetime.Singleton)
        .WithFactory(sp => new MyService(sp.GetRequiredService<ILogger>()))
        .Decorate<IMyService, LoggingDecorator>();
    
    return ValueTask.CompletedTask;
}
```

### 3. Module Lifecycle Fluent API

```csharp
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        this
            .ConfigureServices(context)
            .RegisterOptions<MyOptions>()
            .RegisterServices()
            .RegisterMiddleware();
        
        return ValueTask.CompletedTask;
    }
}
```

---

## ❓ Questions for You

1. **Which approach do you prefer?** (Method chaining, Builder pattern, or Hybrid)
2. **Should configuration be immutable after OnConfigureAsync?**
3. **Do you want configuration accessible in all lifecycle methods?**
4. **Should we support configuration inheritance/composition?**
5. **Do you want automatic configuration binding from IConfiguration?**
6. **Should configuration be queryable across modules?**
7. **Do you want configuration change notifications?**
8. **Should we support configuration profiles/environments?**

---

## 🚀 Next Steps

Once you answer the questions, I'll implement:
1. Fluent API extension methods
2. Module configuration builder
3. Configuration property helpers
4. Validation framework
5. Documentation and examples

