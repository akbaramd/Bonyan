# پیشنهادات بهبود سیستم ماژولار Bonyan

## 📋 خلاصه

این سند پیشنهادات بهبود برای سیستم ماژولار Bonyan را ارائه می‌دهد. این پیشنهادات بر اساس تحلیل معماری فعلی و نیازهای رایج در سیستم‌های ماژولار طراحی شده‌اند.

---

## 🎯 بهبودهای پیشنهادی

### 1. **پشتیبانی از Logging در ماژول‌ها** ⭐⭐⭐

**مشکل فعلی:**
- ماژول‌ها دسترسی آسان به `ILogger` ندارند
- باید از `context.Services` استفاده کنند که پیچیده است

**راه حل:**
```csharp
public abstract class BonModule : IBonModule
{
    // Property برای دسترسی آسان به Logger
    protected ILogger? Logger { get; private set; }
    
    // Property برای دسترسی به LoggerFactory
    protected ILoggerFactory? LoggerFactory { get; private set; }
    
    // Helper method برای ایجاد Logger
    protected ILogger<T> CreateLogger<T>() => LoggerFactory?.CreateLogger<T>();
}
```

**استفاده:**
```csharp
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Logger?.LogInformation("Configuring MyModule");
        // ...
    }
}
```

---

### 2. **اطلاعات ماژول (Module Metadata)** ⭐⭐⭐

**مشکل فعلی:**
- هیچ راه آسانی برای دسترسی به نام، نسخه، یا توضیحات ماژول وجود ندارد

**راه حل:**
```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ModuleInfoAttribute : Attribute
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
}

public abstract class BonModule
{
    // Property برای دسترسی به اطلاعات ماژول
    public ModuleInfo? ModuleInfo { get; protected set; }
    
    protected virtual ModuleInfo GetModuleInfo()
    {
        var attr = GetType().GetCustomAttribute<ModuleInfoAttribute>();
        return new ModuleInfo
        {
            Name = attr?.Name ?? GetType().Name,
            Version = attr?.Version ?? "1.0.0",
            Description = attr?.Description,
            Author = attr?.Author
        };
    }
}
```

**استفاده:**
```csharp
[ModuleInfo(Name = "Payment Module", Version = "2.0.0", Description = "Payment processing")]
public class PaymentModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Logger?.LogInformation("Configuring {ModuleName} v{Version}", 
            ModuleInfo.Name, ModuleInfo.Version);
    }
}
```

---

### 3. **Helper Methods برای Configuration** ⭐⭐

**مشکل فعلی:**
- کد تکراری برای خواندن تنظیمات از `appsettings.json`

**راه حل:**
```csharp
public abstract class BonModule
{
    // Bind configuration from appsettings.json
    protected void ConfigureFromSection<TOptions>(string sectionName)
        where TOptions : class
    {
        Services.Configure<TOptions>(Configuration.GetSection(sectionName));
    }
    
    // Configure with validation
    protected void ConfigureWithValidation<TOptions>(
        Action<TOptions> configure,
        Func<TOptions, bool> validate)
        where TOptions : class, new()
    {
        Services.Configure(configure);
        Services.PostConfigure<TOptions>(options =>
        {
            if (!validate(options))
                throw new OptionsValidationException(...);
        });
    }
    
    // Get configuration value directly
    protected T? GetConfigValue<T>(string key)
    {
        return Configuration.GetValue<T>(key);
    }
    
    // Get required configuration value
    protected T GetRequiredConfigValue<T>(string key)
    {
        return Configuration.GetValue<T>(key) 
            ?? throw new ConfigurationNotFoundException(key);
    }
}
```

---

### 4. **پشتیبانی از Health Checks** ⭐⭐⭐

**مشکل فعلی:**
- هیچ راه استانداردی برای اضافه کردن Health Checks وجود ندارد

**راه حل:**
```csharp
public abstract class BonModule
{
    // Register health check
    protected void AddHealthCheck<T>(string name) where T : class, IHealthCheck
    {
        Services.AddHealthChecks().AddCheck<T>(name);
    }
    
    // Register health check with factory
    protected void AddHealthCheck(string name, Func<IServiceProvider, IHealthCheck> factory)
    {
        Services.AddHealthChecks().Add(new HealthCheckRegistration(name, factory, null, null));
    }
}
```

**استفاده:**
```csharp
public class DatabaseModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        AddHealthCheck<DatabaseHealthCheck>("database");
        return base.OnConfigureAsync(context, ct);
    }
}
```

---

### 5. **پشتیبانی از Background Services** ⭐⭐

**مشکل فعلی:**
- ثبت Background Services پیچیده است

**راه حل:**
```csharp
public abstract class BonModule
{
    // Register background service
    protected void AddBackgroundService<T>() where T : class, IHostedService
    {
        Services.AddHostedService<T>();
    }
    
    // Register background service with factory
    protected void AddBackgroundService(Func<IServiceProvider, IHostedService> factory)
    {
        Services.AddSingleton<IHostedService>(sp => factory(sp));
    }
}
```

---

### 6. **بررسی وجود ماژول‌های وابسته** ⭐

**مشکل فعلی:**
- نمی‌توان بررسی کرد که آیا یک ماژول خاص لود شده است یا نه

**راه حل:**
```csharp
public abstract class BonModule
{
    // Check if a module is loaded
    protected bool IsModuleLoaded<TModule>() where TModule : IBonModule
    {
        var container = Services.GetService<IBonModuleContainer>();
        return container?.Modules.Any(m => m.ModuleType == typeof(TModule)) ?? false;
    }
    
    // Get module descriptor
    protected BonModuleDescriptor? GetModuleDescriptor<TModule>() where TModule : IBonModule
    {
        var container = Services.GetService<IBonModuleContainer>();
        return container?.Modules.FirstOrDefault(m => m.ModuleType == typeof(TModule));
    }
}
```

---

### 7. **Fluent API برای Service Registration** ⭐⭐⭐

**مشکل فعلی:**
- ثبت سرویس‌ها verbose است

**راه حل:**
```csharp
public static class BonModuleServiceRegistrationExtensions
{
    public static BonModule AddService<TService, TImplementation>(
        this BonModule module,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        module.Services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return module;
    }
    
    public static BonModule AddSingleton<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        module.Services.AddSingleton<TService, TImplementation>();
        return module;
    }
    
    public static BonModule AddScoped<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        module.Services.AddScoped<TService, TImplementation>();
        return module;
    }
    
    public static BonModule AddTransient<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        module.Services.AddTransient<TService, TImplementation>();
        return module;
    }
}
```

**استفاده:**
```csharp
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        this
            .AddSingleton<IMyService, MyService>()
            .AddScoped<IRepository, Repository>()
            .AddTransient<IValidator, Validator>();
            
        return base.OnConfigureAsync(context, ct);
    }
}
```

---

### 8. **Environment Helpers** ⭐⭐

**مشکل فعلی:**
- بررسی Environment پیچیده است

**راه حل:**
```csharp
public abstract class BonModule
{
    // Check environment
    protected bool IsDevelopment() => 
        Configuration["ASPNETCORE_ENVIRONMENT"] == "Development";
    
    protected bool IsProduction() => 
        Configuration["ASPNETCORE_ENVIRONMENT"] == "Production";
    
    protected bool IsEnvironment(string environment) => 
        Configuration["ASPNETCORE_ENVIRONMENT"] == environment;
    
    // Get environment name
    protected string GetEnvironment() => 
        Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
}
```

---

### 9. **Configuration Builder Pattern** ⭐⭐

**مشکل فعلی:**
- تنظیمات پیچیده Options سخت است

**راه حل:**
```csharp
public class ModuleOptionsBuilder<TOptions> where TOptions : class, new()
{
    private readonly BonModule _module;
    private readonly TOptions _options = new();
    
    public ModuleOptionsBuilder(BonModule module)
    {
        _module = module;
    }
    
    public ModuleOptionsBuilder<TOptions> Set(Action<TOptions> configure)
    {
        configure(_options);
        return this;
    }
    
    public ModuleOptionsBuilder<TOptions> FromSection(string sectionName)
    {
        _module.Services.Configure<TOptions>(
            _module.Configuration.GetSection(sectionName));
        return this;
    }
    
    public ModuleOptionsBuilder<TOptions> Validate(Func<TOptions, bool> validator)
    {
        _module.Services.PostConfigure<TOptions>(options =>
        {
            if (!validator(options))
                throw new OptionsValidationException(...);
        });
        return this;
    }
    
    public void Register()
    {
        _module.Services.Configure<TOptions>(_ => _options);
    }
}

public abstract class BonModule
{
    protected ModuleOptionsBuilder<TOptions> ConfigureOptions<TOptions>() 
        where TOptions : class, new()
    {
        return new ModuleOptionsBuilder<TOptions>(this);
    }
}
```

**استفاده:**
```csharp
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        ConfigureOptions<MyOptions>()
            .FromSection("MyOptions")
            .Set(options => options.EnableFeature = true)
            .Validate(options => !string.IsNullOrEmpty(options.ApiKey))
            .Register();
            
        return base.OnConfigureAsync(context, ct);
    }
}
```

---

### 10. **Event Bus / Mediator Pattern** ⭐⭐⭐

**مشکل فعلی:**
- هیچ راه استانداردی برای ارتباط بین ماژول‌ها وجود ندارد

**راه حل:**
```csharp
public interface IModuleEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class;
    void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : class;
}

public abstract class BonModule
{
    protected IModuleEventBus? EventBus { get; private set; }
    
    // Publish event
    protected Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken ct = default) 
        where TEvent : class
    {
        return EventBus?.PublishAsync(@event, ct) ?? Task.CompletedTask;
    }
    
    // Subscribe to event
    protected void SubscribeToEvent<TEvent>(Func<TEvent, CancellationToken, Task> handler) 
        where TEvent : class
    {
        EventBus?.Subscribe(handler);
    }
}
```

---

## 📊 اولویت‌بندی

| # | ویژگی | اولویت | پیچیدگی | فایده |
|---|--------|--------|---------|-------|
| 1 | Logging Support | ⭐⭐⭐ | کم | بالا |
| 2 | Module Metadata | ⭐⭐⭐ | کم | متوسط |
| 3 | Configuration Helpers | ⭐⭐ | کم | بالا |
| 4 | Health Checks | ⭐⭐⭐ | متوسط | بالا |
| 5 | Background Services | ⭐⭐ | کم | متوسط |
| 6 | Module Dependency Check | ⭐ | کم | پایین |
| 7 | Fluent Service Registration | ⭐⭐⭐ | کم | بالا |
| 8 | Environment Helpers | ⭐⭐ | کم | متوسط |
| 9 | Configuration Builder | ⭐⭐ | متوسط | متوسط |
| 10 | Event Bus | ⭐⭐⭐ | بالا | بالا |

---

## 🚀 پیشنهاد پیاده‌سازی

### فاز 1 (سریع - فایده بالا)
1. Logging Support
2. Fluent Service Registration
3. Configuration Helpers

### فاز 2 (متوسط)
4. Module Metadata
5. Health Checks
6. Environment Helpers

### فاز 3 (پیشرفته)
7. Event Bus
8. Configuration Builder
9. Background Services

---

## 💡 سوالات برای تصمیم‌گیری

1. آیا می‌خواهید همه این ویژگی‌ها را پیاده‌سازی کنیم؟
2. کدام ویژگی‌ها اولویت بیشتری دارند؟
3. آیا می‌خواهید Event Bus را پیاده‌سازی کنیم؟
4. آیا می‌خواهید Configuration Builder را پیاده‌سازی کنیم؟

