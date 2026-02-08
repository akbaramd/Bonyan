# Bonyan.VirtualFileSystem

## Table of Contents

- [Introduction](#introduction)
- [Dependencies and setup](#dependencies-and-setup)
- [How it works](#how-it-works)
  - [BonVirtualFileSystemOptions and FileSets](#bonvirtualfilesystemoptions-and-filesets)
  - [Resolution order](#resolution-order)
- [Adding file sets (in each module)](#adding-file-sets-in-each-module)
  - [Embedded resources: AddEmbedded](#embedded-resources-addembedded)
  - [Physical folder: AddPhysical](#physical-folder-addphysical)
  - [Override embedded with physical: ReplaceEmbeddedByPhysical](#override-embedded-with-physical-replaceembeddedbyphysical)
- [Using the virtual file system in code](#using-the-virtual-file-system-in-code)
- [Per-module pattern (checklist)](#per-module-pattern-checklist)
- [Summary](#summary)

---

## Introduction

**Bonyan.VirtualFileSystem** provides a single virtual view over multiple file sources:

- **Embedded resources** from assemblies (each module can ship its own files).
- **Physical directories** on disk.
- **Dynamic files** added at runtime via `IDynamicFileProvider`.

Consumers use **IVirtualFileProvider** (same as .NET’s `IFileProvider`): request a path and get `IFileInfo` or `IDirectoryContents`. The framework then looks up that path across all registered file sets (and the dynamic provider) in a defined order.

Typical uses:

- **Localization**: JSON files under a virtual path (e.g. `/Localization/Resources/MyModule/`) are loaded by the localization module.
- **Static assets**: CSS, JS, or other content embedded in modules and served under a virtual path.
- **Templates or config**: Overridable files (embedded by default, physical in development).

---

## Dependencies and setup

1. **Add the package**

```bash
dotnet add package Bonyan.VirtualFileSystem
```

2. **Depend on the module**

Any module that **registers** file sets or **uses** `IVirtualFileProvider` must depend on **BonVirtualFileSystemModule**:

```csharp
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

[DependsOn(typeof(BonVirtualFileSystemModule))]
public class MyModule : BonModule
{
}
```

3. **Registration**

The module registers `IVirtualFileProvider` and `IDynamicFileProvider` in DI. You do **not** register the provider yourself; you only **configure** `BonVirtualFileSystemOptions` (see below).

---

## How it works

### BonVirtualFileSystemOptions and FileSets

All virtual file sources are configured on **BonVirtualFileSystemOptions**:

- **FileSets** – list of static file sources (embedded or physical). You add to this in your module’s `OnConfigureAsync`.

```csharp
public class BonVirtualFileSystemOptions
{
    public VirtualFileSetList FileSets { get; }  // List<VirtualFileSetInfo>
}
```

- The **VirtualFileProvider** (registered as `IVirtualFileProvider`) builds a composite over:
  1. **IDynamicFileProvider** (runtime-added files).
  2. Each **FileSet**, in **reverse** order (last added is consulted first after dynamic).

So: **dynamic** has highest priority, then the **last** file set you added, then the previous one, and so on. Later modules (or later configuration) can “override” earlier ones for the same path.

### Resolution order

When you call `IVirtualFileProvider.GetFileInfo(subpath)`:

1. **DynamicFileProvider** is asked first (runtime files).
2. Then **FileSets** in **reverse** order: last added → first added.

The first provider that returns a **non-“not found”** result wins. So to override a file from an earlier module, add your file set **after** that module’s configuration (or add the file dynamically).

---

## Adding file sets (in each module)

Do this in your module’s **OnConfigureAsync** by configuring **BonVirtualFileSystemOptions** and appending to **FileSets**.

### Embedded resources: AddEmbedded

Expose embedded resources from an assembly as a virtual file set.

**Signature:**

```csharp
public static void AddEmbedded<T>(
    this VirtualFileSetList list,
    string? baseNamespace = null,
    string? baseFolder = null)
```

- **T** – any type in the **assembly that contains the embedded resources**. That assembly is used to create the embedded file provider.
- **baseNamespace** – (optional) used when the assembly does **not** use the SDK-style embedded manifest; only manifest resource names starting with this namespace are included. Namespace is stripped to build the virtual path.
- **baseFolder** – (optional) used with SDK-style **ManifestEmbeddedFileProvider** as the logical root folder for embedded files. Omit only if you don’t use a folder prefix.

**Example 1 – SDK-style embedded (recommended)**

In the project that contains the files (e.g. your module project), embed a folder:

```xml
<ItemGroup>
  <EmbeddedResource Include="Localization\**\*" LinkBase="Localization" />
</ItemGroup>
```

Then in the module:

```csharp
public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
{
    context.Services.Configure<BonVirtualFileSystemOptions>(options =>
    {
        options.FileSets.AddEmbedded<MyModule>(baseFolder: "Localization");
    });
    return base.OnConfigureAsync(context, cancellationToken);
}
```

Files under `Localization\` will be exposed under virtual path `/Localization/...` (e.g. `/Localization/Resources/MyModule/en.json`).

**Example 2 – Legacy embedded (BonEmbeddedFileProvider)**

If the assembly uses classic embedded resources (no manifest), use **baseNamespace** so only resources under that namespace are exposed and their names are converted to paths:

```csharp
options.FileSets.AddEmbedded<MyModule>(baseNamespace: "MyModule.AssemblyName.Resources");
```

**Important:** Paths are normalized: forward slashes, and dots in “folder” parts become slashes. So the virtual path you use in code (e.g. in localization’s `AddVirtualJson`) must match the path derived from your resource names or from the manifest.

### Physical folder: AddPhysical

Expose a folder on disk as a file set.

```csharp
context.Services.Configure<BonVirtualFileSystemOptions>(options =>
{
    options.FileSets.AddPhysical(Path.Combine(AppContext.BaseDirectory, "wwwroot", "Localization"));
});
```

Paths under that root become virtual paths (e.g. `Resources/MyModule/en.json` under that root → `/Resources/MyModule/en.json` or similar, depending on how the physical provider maps paths). Use the same path convention as your consumers (e.g. localization) expect.

### Override embedded with physical: ReplaceEmbeddedByPhysical

In development, you can replace an **embedded** file set from a given assembly with a **physical** folder so you can edit files on disk without rebuilding.

```csharp
context.Services.Configure<BonVirtualFileSystemOptions>(options =>
{
    options.FileSets.ReplaceEmbeddedByPhysical<MyModule>(
        Path.Combine(AppContext.BaseDirectory, "LocalizationOverride"));
});
```

The first file set that was added from `MyModule`’s assembly is replaced by a physical file set. If you used `baseFolder: "Localization"`, the physical path is combined with that folder (e.g. `LocalizationOverride/Localization`). Place your files so the resulting paths match what your code expects (e.g. `/Localization/Resources/MyModule/en.json`).

---

## Using the virtual file system in code

Inject **IVirtualFileProvider** and use it like any .NET `IFileProvider`.

**Get a file:**

```csharp
public class MyService
{
    private readonly IVirtualFileProvider _virtualFileProvider;

    public MyService(IVirtualFileProvider virtualFileProvider)
    {
        _virtualFileProvider = virtualFileProvider;
    }

    public string? ReadConfig(string virtualPath)
    {
        var file = _virtualFileProvider.GetFileInfo(virtualPath);
        if (!file.Exists)
            return null;
        return file.ReadAsString();  // extension from Bonyan (Microsoft.Extensions.FileProviders)
    }
}
```

**Paths:**

- Use forward slashes and a leading slash, e.g. `/Localization/Resources/MyModule/en.json`.
- Paths are normalized (see `VirtualFilePathHelper`). Be consistent with the paths you register (embedded or physical) and the paths you request.

**List a directory:**

```csharp
var contents = _virtualFileProvider.GetDirectoryContents("/Localization/Resources/MyModule");
if (contents.Exists)
{
    foreach (var entry in contents)
    {
        // entry.Name, entry.IsDirectory, entry.CreateReadStream(), etc.
    }
}
```

**Watch for changes:**

```csharp
var token = _virtualFileProvider.Watch("/Localization/Resources/MyModule/*.json");
// RegisterCallback or use with other APIs that accept IChangeToken
```

Embedded file sets return `NullChangeToken` (no change notifications). Dynamic and physical providers can support watching depending on implementation.

---

## Per-module pattern (checklist)

To add your module’s files to the virtual file system and use them correctly:

1. **Reference and depend on the module**
   - Add package `Bonyan.VirtualFileSystem`.
   - `[DependsOn(typeof(BonVirtualFileSystemModule))]` on your module.

2. **Add a file set in your module**
   - In **OnConfigureAsync**: `context.Services.Configure<BonVirtualFileSystemOptions>(options => { ... })`.
   - **Embedded**: `options.FileSets.AddEmbedded<YourModule>(baseNamespace: "...", baseFolder: "...")` and embed the files in the project (e.g. `EmbeddedResource` with `LinkBase`).
   - **Physical**: `options.FileSets.AddPhysical(rootPath)` and use the same path layout under that root.

3. **Use a consistent virtual path**
   - Decide a virtual root for your module (e.g. `/Localization/Resources/MyModule`).
   - Place files (embedded or physical) so they appear at that path.
   - In code (or in localization’s `AddVirtualJson`, etc.), use that same path.

4. **Consume via IVirtualFileProvider**
   - Inject `IVirtualFileProvider` and call `GetFileInfo(path)` or `GetDirectoryContents(path)` with the same path convention.

5. **(Optional) Development override**
   - Use `ReplaceEmbeddedByPhysical<YourModule>(physicalPath)` so the same virtual paths are served from disk instead of embedded.

---

## Summary

- **BonVirtualFileSystemOptions.FileSets** is the list of static file sources; you add to it in each module that ships or exposes files.
- **AddEmbedded<T>**(baseNamespace, baseFolder) exposes embedded resources from T’s assembly; use **baseFolder** for SDK-style embedding.
- **AddPhysical**(root) exposes a physical directory.
- **ReplaceEmbeddedByPhysical<T>**(path) swaps that assembly’s embedded set for a physical folder (e.g. for development).
- Resolution order: **Dynamic** → **FileSets in reverse order**. Later file sets override earlier ones for the same path.
- Depend on **BonVirtualFileSystemModule** and configure **BonVirtualFileSystemOptions** in **OnConfigureAsync**; then use **IVirtualFileProvider** with consistent virtual paths across registration and consumption.

Using this, each module can add its own file set and the virtual file system will merge them into one predictable tree for localization, static files, or other features.
