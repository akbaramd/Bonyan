# Bonyan.AspNetCore.Localization

ASP.NET Core specific localization module for the Bonyan framework that provides DI registration, service collection configuration, and ASP.NET Core integration for localization features.

## Architecture

This module depends on `Bonyan.Localization` (core module) and provides the actual implementation for:
- Service registration in DI container
- ASP.NET Core integration
- Request localization middleware
- Resource file handling

## Features

- **Easy Configuration**: Simple configuration through options pattern
- **Multiple Culture Support**: Support for multiple cultures with easy configuration
- **Resource Management**: Centralized resource management
- **Extension Methods**: Fluent API for configuration
- **Service Integration**: Seamless integration with Bonyan framework
- **Request Localization**: Automatic request culture detection
- **View Localization**: Support for view localization
- **Data Annotations Localization**: Support for data annotations localization

## Installation

This module depends on `Bonyan.Localization` and should be added to your ASP.NET Core application. The core localization module (`Bonyan.Localization`) is used by all modules that need localization support.

## Configuration

### Basic Configuration

```csharp
// In your module configuration
public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
{
    context.Services.Configure<BonLocalizationOptions>(options =>
    {
        options.SupportedCultures = new[] { "en-US", "fa-IR", "ar-SA" };
        options.SupportedUICultures = new[] { "en-US", "fa-IR", "ar-SA" };
        options.DefaultCulture = "fa-IR";
        options.DefaultUICulture = "fa-IR";
        options.ResourcesPath = "Resources";
    });
    
    return base.OnConfigureAsync(context);
}
```

### Using Extension Methods

```csharp
// In Program.cs or Startup.cs
services.AddBonLocalization(options =>
{
    options.AddSupportedCulture("en-US")
           .AddSupportedCulture("fa-IR")
           .AddSupportedCulture("ar-SA")
           .SetDefaultCulture("fa-IR")
           .SetResourcesPath("Localization");
});
```

### Application Configuration

```csharp
// In Program.cs or Startup.cs
app.UseBonLocalization();
```

## Usage

### In Controllers

```csharp
public class HomeController : Controller
{
    private readonly IBonLocalizationService _localizationService;
    
    public HomeController(IBonLocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    
    public IActionResult Index()
    {
        var welcomeMessage = _localizationService.L("WelcomeMessage");
        var greetingMessage = _localizationService.L("GreetingMessage", "Hello {0}!", User.Identity.Name);
        
        return View();
    }
}
```

### In Views

```csharp
@inject IBonLocalizationService LocalizationService

<h1>@LocalizationService.L("WelcomeTitle")</h1>
<p>@LocalizationService.L("WelcomeMessage")</p>
```

### Using String Localizer

```csharp
public class MyService
{
    private readonly IStringLocalizer<MyService> _localizer;
    
    public MyService(IStringLocalizer<MyService> localizer)
    {
        _localizer = localizer;
    }
    
    public string GetMessage(string key)
    {
        return _localizer[key];
    }
}
```

## Resource Files

Create resource files in your `Resources` folder (or custom path):

### SharedResource.resx (Default)
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="WelcomeMessage" xml:space="preserve">
    <value>Welcome to our application!</value>
  </data>
  <data name="GreetingMessage" xml:space="preserve">
    <value>Hello {0}!</value>
  </data>
</root>
```

### SharedResource.fa-IR.resx (Persian)
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="WelcomeMessage" xml:space="preserve">
    <value>به برنامه ما خوش آمدید!</value>
  </data>
  <data name="GreetingMessage" xml:space="preserve">
    <value>سلام {0}!</value>
  </data>
</root>
```

## Advanced Configuration

### Custom Configurators

```csharp
services.AddBonLocalization(options =>
{
    options.AddConfigurator(services =>
    {
        // Add custom localization services
        services.AddSingleton<ICustomLocalizationProvider, CustomLocalizationProvider>();
    });
});
```

### Disabling Features

```csharp
services.AddBonLocalization(options =>
{
    options.DisableRequestLocalization();
    options.DisableDataAnnotationsLocalization();
});
```

## Services

### IBonLocalizationService

Main service for localization operations:

- `L(string name)` - Get localized string
- `L(string name, params object[] arguments)` - Get localized string with parameters
- `L(string name, string defaultValue)` - Get localized string with default value
- `GetLocalizer<T>()` - Get string localizer for specific type
- `GetCurrentCulture()` - Get current culture
- `SetCulture(string culture)` - Set current culture

### IBonLocalizationManager

Lower-level localization management:

- `GetString(string name)` - Get localized string
- `GetLocalizer<T>()` - Get string localizer
- `AddLocalizationProvider<T>()` - Add custom localization provider

## Options

### BonLocalizationOptions

- `SupportedCultures` - Array of supported cultures
- `SupportedUICultures` - Array of supported UI cultures
- `DefaultCulture` - Default culture
- `DefaultUICulture` - Default UI culture
- `ResourcesPath` - Path to resource files
- `EnableRequestLocalization` - Enable request localization
- `EnableViewLocalization` - Enable view localization
- `EnableDataAnnotationsLocalization` - Enable data annotations localization

## Dependencies

- Microsoft.Extensions.Localization
- Microsoft.AspNetCore.Localization
- Bonyan (Core framework)
- Bonyan.AspNetCore (ASP.NET Core integration)
- Bonyan.Localization (Core localization module) 