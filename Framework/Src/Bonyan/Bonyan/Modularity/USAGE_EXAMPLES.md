# مثال‌های استفاده از بهبودهای ماژولار

## 📚 مثال‌های عملی

### مثال 1: استفاده از BonModuleEnhanced با Logging

```csharp
using Bonyan.Modularity.Abstractions;

[ModuleInfo(Name = "Payment Module", Version = "2.0.0", Description = "Payment processing module")]
public class PaymentModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Logger به صورت خودکار در دسترس است
        Logger?.LogInformation("Configuring {ModuleName} v{Version}", 
            ModuleInfo?.Name, ModuleInfo?.Version);

        // استفاده از Fluent API برای ثبت سرویس‌ها
        this
            .AddSingleton<IPaymentService, PaymentService>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddTransient<IPaymentValidator, PaymentValidator>();

        // تنظیمات از appsettings.json
        ConfigureFromSection<PaymentOptions>("Payment");

        // تنظیمات با validation
        ConfigureWithValidation<PaymentSecurityOptions>(
            configure: options => options.EnableEncryption = true,
            validate: options => !string.IsNullOrEmpty(options.EncryptionKey));

        Logger?.LogInformation("Payment module configured successfully");
        
        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Initializing Payment module");
        
        // استفاده از configuration
        var paymentOptions = context.GetOption<PaymentOptions>();
        if (paymentOptions?.EnableFeature == true)
        {
            Logger?.LogInformation("Payment feature is enabled");
        }
        
        return base.OnInitializeAsync(context, cancellationToken);
    }
}
```

---

### مثال 2: استفاده از Health Checks

```csharp
[ModuleInfo(Name = "Database Module", Version = "1.0.0")]
public class DatabaseModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Configuring Database module");

        // ثبت Health Check
        AddHealthCheck<DatabaseHealthCheck>("database");
        
        // یا با factory
        AddHealthCheck("database-custom", sp => 
            new DatabaseHealthCheck(sp.GetRequiredService<IDbConnection>()));

        // ثبت سرویس‌ها
        this
            .AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
            .AddScoped<IDbContext, DbContext>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

---

### مثال 3: استفاده از Background Services

```csharp
[ModuleInfo(Name = "Notification Module", Version = "1.0.0")]
public class NotificationModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Configuring Notification module");

        // ثبت Background Service
        AddBackgroundService<EmailNotificationService>();
        
        // یا با factory
        AddBackgroundService(sp => 
            new ScheduledNotificationService(
                sp.GetRequiredService<ILogger<ScheduledNotificationService>>()));

        this
            .AddSingleton<IEmailService, EmailService>()
            .AddScoped<INotificationRepository, NotificationRepository>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

---

### مثال 4: بررسی Environment و Configuration

```csharp
[ModuleInfo(Name = "Feature Module", Version = "1.0.0")]
public class FeatureModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // بررسی Environment
        if (IsDevelopment())
        {
            Logger?.LogInformation("Running in Development mode - enabling debug features");
            this.AddSingleton<IDebugService, DebugService>();
        }

        if (IsProduction())
        {
            Logger?.LogInformation("Running in Production mode - enabling production features");
        }

        // خواندن مقادیر از Configuration
        var apiKey = GetConfigValue<string>("ApiKeys:Main");
        var maxRetries = GetRequiredConfigValue<int>("Settings:MaxRetries");

        // تنظیمات شرطی
        if (IsEnvironment("Staging"))
        {
            ConfigureFromSection<StagingOptions>("Staging");
        }

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

---

### مثال 5: بررسی وجود ماژول‌های وابسته

```csharp
[ModuleInfo(Name = "Advanced Module", Version = "1.0.0")]
[DependsOn(typeof(DatabaseModule))]
public class AdvancedModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // بررسی اینکه آیا DatabaseModule لود شده است
        if (IsModuleLoaded<DatabaseModule>())
        {
            Logger?.LogInformation("DatabaseModule is loaded - using database features");
            
            var dbModule = GetModuleDescriptor<DatabaseModule>();
            Logger?.LogInformation("Database module version: {Version}", 
                dbModule?.ModuleType.Assembly.GetName().Version);
        }
        else
        {
            Logger?.LogWarning("DatabaseModule is not loaded - some features may not work");
        }

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

---

### مثال 6: استفاده از Fluent API برای Service Registration

```csharp
[ModuleInfo(Name = "Service Module", Version = "1.0.0")]
public class ServiceModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // Fluent API - زنجیره‌ای و خوانا
        this
            .AddSingleton<ICacheService, CacheService>()
            .AddSingleton<ICacheService>(sp => new AdvancedCacheService(
                sp.GetRequiredService<ILogger<AdvancedCacheService>>()))
            .AddScoped<IRepository, Repository>()
            .AddScoped(sp => new CustomRepository(
                sp.GetRequiredService<IDbContext>()))
            .AddTransient<IValidator, Validator>()
            .AddAs<MultiInterfaceService>(typeof(IService1), typeof(IService2), typeof(IService3))
            .Decorate<IRepository, CachedRepository>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

---

### مثال 7: ترکیب همه ویژگی‌ها

```csharp
[ModuleInfo(
    Name = "Complete Module", 
    Version = "1.0.0", 
    Description = "A complete example module",
    Author = "Bonyan Team")]
[DependsOn(typeof(DatabaseModule))]
public class CompleteModule : BonModuleEnhanced
{
    public override ValueTask OnPreConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Pre-configuring {ModuleName}", ModuleInfo?.Name);
        
        // Early configuration
        ConfigureFromSection<EarlyOptions>("Early");
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Configuring {ModuleName} v{Version}", 
            ModuleInfo?.Name, ModuleInfo?.Version);

        // Environment-specific configuration
        if (IsDevelopment())
        {
            Logger?.LogInformation("Development mode - enabling debug features");
        }

        // Service registration with fluent API
        this
            .AddSingleton<IMainService, MainService>()
            .AddScoped<ISubService, SubService>()
            .AddTransient<IHelper, Helper>();

        // Configuration with validation
        ConfigureWithValidation<SecurityOptions>(
            configure: options => 
            {
                options.EnableSSL = true;
                options.Timeout = 30;
            },
            validate: options => 
                options.Timeout > 0 && options.Timeout <= 300);

        // Health checks
        AddHealthCheck<ServiceHealthCheck>("complete-module");

        // Background services
        AddBackgroundService<BackgroundWorkerService>();

        // Check dependencies
        if (IsModuleLoaded<DatabaseModule>())
        {
            Logger?.LogInformation("Database module is available");
        }

        Logger?.LogInformation("{ModuleName} configured successfully", ModuleInfo?.Name);
        
        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnPostConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Post-configuring {ModuleName}", ModuleInfo?.Name);
        
        // Final adjustments
        var config = GetConfigValue<string>("FinalSettings:Value");
        Logger?.LogInformation("Final configuration value: {Value}", config);
        
        return base.OnPostConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("Initializing {ModuleName}", ModuleInfo?.Name);
        
        // Use services from context
        var service = context.GetService<IMainService>();
        if (service != null)
        {
            Logger?.LogInformation("Main service is available");
        }
        
        return base.OnInitializeAsync(context, cancellationToken);
    }
}
```

---

## 🎯 نکات مهم

1. **BonModuleEnhanced** را به جای `BonModule` استفاده کنید تا به همه ویژگی‌ها دسترسی داشته باشید
2. **Logger** به صورت خودکار در دسترس است - نیازی به ثبت دستی نیست
3. **Fluent API** برای ثبت سرویس‌ها بسیار خوانا و زنجیره‌ای است
4. **ModuleInfo** برای مستندسازی و logging مفید است
5. **Health Checks** و **Background Services** به راحتی قابل اضافه کردن هستند

---

## 📝 Migration Guide

برای تبدیل ماژول‌های موجود به `BonModuleEnhanced`:

```csharp
// قبل
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        context.Services.AddSingleton<IMyService, MyService>();
        return base.OnConfigureAsync(context, ct);
    }
}

// بعد
public class MyModule : BonModuleEnhanced
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken ct)
    {
        Logger?.LogInformation("Configuring MyModule");
        
        this.AddSingleton<IMyService, MyService>();
        
        return base.OnConfigureAsync(context, ct);
    }
}
```

