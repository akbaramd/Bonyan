# توضیح `ExecutePreConfiguredActions` و نحوه استفاده

## 📖 مفهوم کلی

`ExecutePreConfiguredActions` یک الگوی **Deferred Configuration** است که به ماژول‌ها اجازه می‌دهد قبل از استفاده از options، آن‌ها را از چندین منبع جمع‌آوری و اعمال کنند.

## 🔄 چرخه کار

```
1. PreConfigure (جمع‌آوری تنظیمات)
   ↓
2. ExecutePreConfiguredActions (اعمال همه تنظیمات)
   ↓
3. استفاده از options
```

## 💡 مثال استفاده در `BonAspNetCoreMvcModule`

```csharp
// در OnConfigureAsync:
var mvcDataAnnotationsLocalizationOptions = context.Services
    .ExecutePreConfiguredActions(
        new BonMvcDataAnnotationsLocalizationOptions()
    );
```

### چه اتفاقی می‌افتد؟

1. یک instance جدید از `BonMvcDataAnnotationsLocalizationOptions` ساخته می‌شود
2. همه `PreConfigure` actions که قبلاً ثبت شده‌اند، روی این instance اجرا می‌شوند
3. instance نهایی با همه تنظیمات اعمال شده برمی‌گردد

## 🎯 روش‌های استفاده

### روش 1: PreConfigure در OnPreConfigureAsync (توصیه شده)

```csharp
public class MyModule : BonModule
{
    public override ValueTask OnPreConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        // ثبت تنظیمات برای بعد (deferred)
        context.Services.PreConfigure<BonMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(MyResources), typeof(MyModule).Assembly);
        });
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }
    
    public override ValueTask OnConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        // اجرای همه PreConfigure actions و دریافت options نهایی
        var options = context.Services.ExecutePreConfiguredActions(
            new BonMvcDataAnnotationsLocalizationOptions()
        );
        
        // استفاده از options
        context.Services.AddMvc()
            .AddDataAnnotationsLocalization(opts =>
            {
                opts.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var resourceType = options.AssemblyResources.GetOrDefault(type.Assembly);
                    return resourceType != null 
                        ? factory.Create(resourceType) 
                        : factory.Create(type);
                };
            });
        
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

### روش 2: PreConfigure در OnConfigureAsync (قبل از Execute)

```csharp
public override ValueTask OnConfigureAsync(
    BonConfigurationContext context, 
    CancellationToken cancellationToken = default)
{
    // 1. ثبت PreConfigure
    context.Services.PreConfigure<MyOptions>(options =>
    {
        options.Setting1 = "Value1";
    });
    
    context.Services.PreConfigure<MyOptions>(options =>
    {
        options.Setting2 = "Value2";
    });
    
    // 2. اجرای همه PreConfigure actions
    var finalOptions = context.Services.ExecutePreConfiguredActions(
        new MyOptions()
    );
    
    // 3. استفاده از finalOptions
    context.Services.Configure<MyOptions>(opts =>
    {
        opts.Setting1 = finalOptions.Setting1;
        opts.Setting2 = finalOptions.Setting2;
    });
    
    return base.OnConfigureAsync(context, cancellationToken);
}
```

### روش 3: استفاده از PreConfigure در Module Base Class

```csharp
public class MyModule : BonModule
{
    public override ValueTask OnPreConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        // استفاده از helper method در BonModule
        context.Services.PreConfigure<BonMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(MyResources), 
                typeof(MyModule).Assembly
            );
        });
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }
}
```

### روش 4: PreConfigure از چندین ماژول

```csharp
// Module A
public class ModuleA : BonModule
{
    public override ValueTask OnPreConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        context.Services.PreConfigure<SharedOptions>(options =>
        {
            options.ModuleA_Setting = "Value from Module A";
        });
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }
}

// Module B
public class ModuleB : BonModule
{
    public override ValueTask OnPreConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        context.Services.PreConfigure<SharedOptions>(options =>
        {
            options.ModuleB_Setting = "Value from Module B";
        });
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }
}

// Module C (که به A و B وابسته است)
public class ModuleC : BonModule
{
    public override ValueTask OnConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        // همه PreConfigure actions از Module A و B اجرا می‌شوند
        var options = context.Services.ExecutePreConfiguredActions(
            new SharedOptions()
        );
        
        // options.ModuleA_Setting و options.ModuleB_Setting هر دو موجود هستند
        context.Services.Configure<SharedOptions>(opts =>
        {
            opts.ModuleA_Setting = options.ModuleA_Setting;
            opts.ModuleB_Setting = options.ModuleB_Setting;
        });
        
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

## 🔍 تفاوت با `Configure`

| روش | زمان اجرا | ترتیب | استفاده |
|-----|-----------|-------|---------|
| **PreConfigure** | قبل از Execute | جمع‌آوری می‌شود | برای جمع‌آوری تنظیمات از چندین ماژول |
| **ExecutePreConfiguredActions** | در OnConfigureAsync | همه PreConfigure ها اجرا می‌شوند | برای دریافت options نهایی |
| **Configure** | در OnConfigureAsync | مستقیماً اعمال می‌شود | برای تنظیم نهایی options |

## 📝 مثال واقعی: BonMvcDataAnnotationsLocalizationOptions

```csharp
// در یک ماژول دیگر (مثلاً LocalizationModule)
public class LocalizationModule : BonModule
{
    public override ValueTask OnPreConfigureAsync(
        BonConfigurationContext context, 
        CancellationToken cancellationToken = default)
    {
        // ثبت resource type برای assembly
        context.Services.PreConfigure<BonMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(SharedResources), 
                typeof(LocalizationModule).Assembly
            );
        });
        
        return base.OnPreConfigureAsync(context, cancellationToken);
    }
}

// در BonAspNetCoreMvcModule
public override ValueTask OnConfigureAsync(
    BonConfigurationContext context, 
    CancellationToken cancellationToken = default)
{
    // همه PreConfigure actions از LocalizationModule و سایر ماژول‌ها اجرا می‌شوند
    var localizationOptions = context.Services
        .ExecutePreConfiguredActions(
            new BonMvcDataAnnotationsLocalizationOptions()
        );
    
    // استفاده از localizationOptions که شامل همه تنظیمات است
    context.Services.AddMvc()
        .AddDataAnnotationsLocalization(opts =>
        {
            opts.DataAnnotationLocalizerProvider = (type, factory) =>
            {
                var resourceType = localizationOptions
                    .AssemblyResources
                    .GetOrDefault(type.Assembly);
                
                return resourceType != null 
                    ? factory.Create(resourceType) 
                    : factory.Create(type);
            };
        });
    
    return base.OnConfigureAsync(context, cancellationToken);
}
```

## ✅ مزایا

1. **جمع‌آوری تنظیمات از چندین ماژول**: ماژول‌های مختلف می‌توانند تنظیمات را اضافه کنند
2. **ترتیب اجرا**: PreConfigure در OnPreConfigureAsync، Execute در OnConfigureAsync
3. **انعطاف‌پذیری**: می‌توانید تنظیمات را از ماژول‌های مختلف جمع‌آوری کنید
4. **عدم وابستگی**: ماژول‌ها نیازی به دانستن وجود یکدیگر ندارند

## 🎓 خلاصه

- **PreConfigure**: برای ثبت تنظیمات (در OnPreConfigureAsync)
- **ExecutePreConfiguredActions**: برای اجرای همه تنظیمات و دریافت options نهایی (در OnConfigureAsync)
- **Configure**: برای تنظیم نهایی options در DI container

