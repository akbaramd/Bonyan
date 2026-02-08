# Bonyan.Localization

## Table of Contents

- [Introduction](#introduction)
- [Dependencies (what you must add)](#dependencies-what-you-must-add)
- [Installation](#installation)
- [How to use](#how-to-use)
  - [IStringLocalizer by resource type](#istringlocalizer-by-resource-type)
  - [IStringLocalizer by resource name](#istringlocalizer-by-resource-name)
  - [Default resource](#default-resource)
- [Adding a localization resource (per module)](#adding-a-localization-resource-per-module)
  - [Step 1: Abstractions (optional)](#step-1-abstractions-optional)
  - [Step 2: Resource class and JSON folder](#step-2-resource-class-and-json-folder)
  - [Step 3: Register resource and virtual files in your module](#step-3-register-resource-and-virtual-files-in-your-module)
- [JSON file format](#json-file-format)
- [Inheriting from another resource](#inheriting-from-another-resource)
- [Summary](#summary)

---

## Introduction

Bonyan.Localization extends .NET localization with:

- **Resource-based model**: Register named resources (backed by JSON or other contributors) and resolve `IStringLocalizer` by type or name.
- **Virtual file system**: Load JSON localization files from embedded resources or physical folders, so each module can ship its own resource folder.
- **Fallback and base resources**: Support default culture, base culture, and inheriting keys from another resource.

You need two packages:

- **Bonyan.Localization.Abstractions** – defines `[LocalizationResourceName]` and localizable string types; no runtime behavior.
- **Bonyan.Localization** – implements resources, virtual JSON loading, and replaces `IStringLocalizerFactory` with the Bonyan implementation.

---

## Dependencies (what you must add)

To use localization in your application or module:

1. **Depend on the module that provides localization:**
   - **BonLocalizationModule** (from package `Bonyan.Localization`) – required for runtime (resources, virtual files, `IStringLocalizerFactory`).
   - **BonLocalizationAbstractionsModule** (from package `Bonyan.Localization.Abstractions`) – required if you use `[LocalizationResourceName]` or other abstractions.  
   Note: `BonLocalizationModule` already depends on `BonLocalizationAbstractionsModule`, so adding `BonLocalizationModule` is enough in most cases.

2. **Module dependency in code:**

```csharp
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

[DependsOn(typeof(BonLocalizationModule))]  // This pulls in BonLocalizationAbstractionsModule
public class MyApplicationModule : BonModule
{
}
```

If your project only references abstractions (e.g. a shared contract assembly that defines resource types) and does not host the localization runtime, depend only on **BonLocalizationAbstractionsModule**:

```csharp
[DependsOn(typeof(BonLocalizationAbstractionsModule))]
public class MyContractsModule : BonModule
{
}
```

---

## Installation

```bash
# Runtime + integration with virtual file system and IStringLocalizer
dotnet add package Bonyan.Localization

# Abstractions only (resource name attribute, localizable strings)
dotnet add package Bonyan.Localization.Abstractions
```

Ensure your app or host module depends on **BonLocalizationModule** so that resources and the custom `IStringLocalizerFactory` are registered.

---

## How to use

### IStringLocalizer by resource type

Register a resource with a type (see [Adding a localization resource](#adding-a-localization-resource-per-module)), then inject `IStringLocalizer<TResource>`:

```csharp
public class MyService
{
    private readonly IStringLocalizer<MyModuleResource> _localizer;

    public MyService(IStringLocalizer<MyModuleResource> localizer)
    {
        _localizer = localizer;
    }

    public string GetMessage()
    {
        return _localizer["WelcomeMessage"];
    }

    public string GetGreeting(string name)
    {
        return _localizer["Greeting", name];
    }
}
```

Keys are the same as in your JSON `texts` (e.g. `"WelcomeMessage"`, `"Greeting"`). The current UI culture is used automatically.

### IStringLocalizer by resource name

If you have a resource registered by name (or from an external store), use `IBonStringLocalizerFactory`:

```csharp
public class MyService
{
    private readonly IBonStringLocalizerFactory _localizerFactory;

    public MyService(IBonStringLocalizerFactory localizerFactory)
    {
        _localizerFactory = localizerFactory;
    }

    public async Task<string> GetStringAsync()
    {
        var localizer = await _localizerFactory.CreateByResourceNameOrNullAsync("MyResourceName");
        return localizer?["Key"] ?? "Key";
    }
}
```

### Default resource

Set `BonLocalizationOptions.DefaultResourceType` so that when a localization call does not specify a resource, the framework uses that type. You can then use `IStringLocalizerFactory.CreateDefaultOrNull()` or the default resource type in `IStringLocalizer<T>`.

---

## Adding a localization resource (per module)

Each module can define its own resource and ship JSON files (embedded or physical). Follow these steps.

### Step 1: Abstractions (optional)

In a shared or abstractions project that references **Bonyan.Localization.Abstractions**, you can define the resource type so other modules can reference it:

```csharp
using Bonyan.Localization;

[LocalizationResourceName("MyModule")]
public class MyModuleResource
{
}
```

The name `"MyModule"` is the resource name used when registering and when resolving by name.

### Step 2: Resource class and JSON folder

In the module that owns the resource (and the JSON files):

1. **Resource class** (if not in abstractions):

```csharp
namespace MyModule.Localization;

[LocalizationResourceName("MyModule")]
public class MyModuleResource
{
}
```

2. **JSON files** per culture in a folder, e.g.:

- **Embedded**: e.g. `Localization/Resources/MyModule/en.json`, `fa.json`, etc., and set that folder as embedded in the project.
- **Physical**: same structure on disk and expose it via the virtual file system (see below).

Example **en.json**:

```json
{
  "culture": "en",
  "texts": {
    "WelcomeMessage": "Welcome",
    "Greeting": "Hello, {0}!"
  }
}
```

### Step 3: Register resource and virtual files in your module

In your module’s `OnConfigureAsync`, do two things:

1. **Add the resource** to `BonLocalizationOptions.Resources` and attach the **virtual JSON** contributor with the path where your JSON files are exposed in the virtual file system.
2. **Register the virtual file set** so that path is populated (embedded or physical).

Example for **embedded** JSON in the same assembly as the module:

```csharp
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.VirtualFileSystem;

[DependsOn(typeof(BonLocalizationModule))]
public class MyModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.Configure<BonVirtualFileSystemOptions>(options =>
        {
            // Expose embedded files; baseFolder maps to folder in assembly (e.g. "Localization").
            options.FileSets.AddEmbedded<MyModule>(baseFolder: "Localization");
        });

        context.Services.Configure<BonLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MyModuleResource>("en")  // default culture for this resource
                .AddVirtualJson("/Localization/Resources/MyModule");
        });

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
```

- `AddEmbedded<MyModule>(baseFolder: "Localization")` makes files under the `Localization` folder in the assembly available in the virtual file system (exact path depends on your project layout; typically you’d have a folder like `Localization/Resources/MyModule/` with `en.json`, `fa.json`, etc.).
- `AddVirtualJson("/Localization/Resources/MyModule")` tells the localization system to load all JSON files from that virtual path for this resource. File names (e.g. `en.json`, `fa.json`) define the culture.

Example layout in the project:

```
MyModule/
  Localization/
    Resources/
      MyModule/
        en.json
        fa.json
```

Ensure these files are included as **Embedded Resource** in the `.csproj` if you use embedded:

```xml
<ItemGroup>
  <EmbeddedResource Include="Localization\Resources\MyModule\*.json" />
</ItemGroup>
```

If you use **physical** files (e.g. on disk at runtime), add that path to the virtual file system instead of (or in addition to) embedded:

```csharp
context.Services.Configure<BonVirtualFileSystemOptions>(options =>
{
    options.FileSets.AddPhysical(Path.Combine(AppContext.BaseDirectory, "Localization"));
});
```

Then `AddVirtualJson("/Localization/Resources/MyModule")` will resolve to that physical path.

---

## JSON file format

Each JSON file must have:

- **culture**: Culture name (e.g. `"en"`, `"en-US"`, `"fa"`).
- **texts**: Object where keys are localization keys and values are the translated strings. Use `{0}`, `{1}`, etc. for parameters.

```json
{
  "culture": "en",
  "texts": {
    "Key": "Value",
    "MessageWithParam": "Hello, {0}!",
    "Nested:Key": "Nested value"
  }
}
```

Keys can be simple (`"Key"`) or nested with a colon (`"Section:Key"`). The contributor loads all JSON files under the virtual path and uses the filename (without extension) as the culture when the file’s `culture` property is not used.

---

## Inheriting from another resource

A resource can fall back to another resource for missing keys:

```csharp
options.Resources
    .Add<MyModuleResource>("en")
    .AddVirtualJson("/Localization/Resources/MyModule")
    .AddBaseResources("BonLocalization");  // Fall back to BonLocalization keys
```

Or by type:

```csharp
options.Resources
    .Add<MyModuleResource>("en")
    .AddVirtualJson("/Localization/Resources/MyModule")
    .AddBaseTypes(typeof(BonLocalizationResource));
```

---

## Summary

- **Dependencies**: Add **BonLocalizationModule** (and optionally **BonLocalizationAbstractionsModule** if you only need abstractions). Your module must `DependsOn(typeof(BonLocalizationModule))` to use resources and virtual JSON.
- **Usage**: Inject `IStringLocalizer<YourResource>` or `IBonStringLocalizerFactory` and use standard `IStringLocalizer` indexing and formatting.
- **Per-module resources**: Define a class with `[LocalizationResourceName("...")]`, add JSON files (embedded or physical), register the virtual file set (see [Virtual File System](/frameworks/virtual-file-system.md)), then `options.Resources.Add<YourResource>("en").AddVirtualJson("/Localization/Resources/YourName")`.
- **JSON**: `{ "culture": "...", "texts": { "Key": "Value" } }`; keys support parameters with `{0}`, `{1}`.

With this, each module can ship its own resource folder and load it through the same virtual path and resource registration pattern.
