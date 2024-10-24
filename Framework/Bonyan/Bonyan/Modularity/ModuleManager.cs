using System.Reflection;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;

namespace Bonyan.Modularity;

public class ModuleManager : IModuleManager
{
    private readonly Dictionary<Type, ModuleInfo> _modules = new();

    public void LoadModules(Type mainModuleType)
    {
        ValidateMainModuleType(mainModuleType);
        LoadModuleRecursive(mainModuleType);
        var sortedModules = TopologicalSort(_modules.Values.ToList());
        InitializeModules(sortedModules);
        UpdateModuleList(sortedModules);
        PrintModuleHierarchy(_modules[mainModuleType], 0, new HashSet<Type>());
    }

    private void ValidateMainModuleType(Type mainModuleType)
    {
        if (mainModuleType == null)
            throw new ArgumentNullException(nameof(mainModuleType));

        if (!typeof(IModule).IsAssignableFrom(mainModuleType))
            throw new ArgumentException($"Type {mainModuleType.FullName} does not implement IModule.");
    }

    private void LoadModuleRecursive(Type moduleType)
    {
        if (_modules.ContainsKey(moduleType)) return;

        var moduleInfo = new ModuleInfo(moduleType);
        _modules[moduleType] = moduleInfo;

        foreach (var dependencyType in GetDependencies(moduleType))
        {
            LoadModuleRecursive(dependencyType);
            moduleInfo.Dependencies.Add(_modules[dependencyType]);
        }
    }

    private IEnumerable<Type> GetDependencies(Type moduleType)
    {
        return moduleType.GetCustomAttributes<DependOnAttribute>()
                         .SelectMany(attr => attr.DependedTypes)
                         .Where(dependencyType => typeof(IModule).IsAssignableFrom(dependencyType));
    }

    private List<ModuleInfo> TopologicalSort(List<ModuleInfo> modules)
    {
        var sorted = new List<ModuleInfo>();
        var visited = new Dictionary<ModuleInfo, bool>();

        foreach (var module in modules)
        {
            if (!Visit(module, visited, sorted))
                throw new InvalidOperationException("Circular dependency detected.");
        }

        return sorted;
    }

    private bool Visit(ModuleInfo module, Dictionary<ModuleInfo, bool> visited, List<ModuleInfo> sorted)
    {
        if (visited.TryGetValue(module, out var inProcess))
        {
            if (inProcess) return false;
            return true;
        }

        visited[module] = true;

        foreach (var dependency in module.Dependencies)
        {
            if (!Visit(dependency, visited, sorted)) return false;
        }

        visited[module] = false;
        sorted.Add(module);
        return true;
    }

    private void InitializeModules(IEnumerable<ModuleInfo> sortedModules)
    {
        foreach (var moduleInfo in sortedModules)
        {
            moduleInfo.Instance = (IModule)Activator.CreateInstance(moduleInfo.ModuleType)!;
            moduleInfo.IsLoaded = true;
        }
    }

    private void UpdateModuleList(IEnumerable<ModuleInfo> sortedModules)
    {
        _modules.Clear();
        foreach (var module in sortedModules)
        {
            _modules[module.ModuleType] = module;
        }
    }

    public async Task ConfigureModulesAsync(ModularityContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnPreConfigureAsync(ctx));
        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnConfigureAsync(ctx));
        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnPostConfigureAsync(ctx));
    }
    // public async Task ConfigureModulesAsync(BonyanApplication app)
    // {
    //   await ExecuteModuleLifecycleAsync(app, async (module, ctx) =>
    //   {
    //     if (module is IWebModule webModule)
    //     {
    //       await webModule.OnPreApplicationAsync(ctx);
    //     }
    //   });
    //   
    //   await ExecuteModuleLifecycleAsync(app, async (module, ctx) =>
    //   {
    //     if (module is IWebModule webModule)
    //     {
    //       await webModule.OnApplicationAsync(ctx);
    //     }
    //   });
    //   
    //   await ExecuteModuleLifecycleAsync(app, async (module, ctx) =>
    //   {
    //     if (module is IWebModule webModule)
    //     {
    //       await webModule.OnPostApplicationAsync(ctx);
    //     }
    //   });
    // }
    public async Task InitializeModulesAsync(ModularityInitializedContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnPreInitializeAsync(ctx));
        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnInitializeAsync(ctx));
        await ExecuteModuleLifecycleAsync(context, (module, ctx) => module.OnPostInitializeAsync(ctx));
    }

    private async Task ExecuteModuleLifecycleAsync<TContext>(TContext context, Func<IModule, TContext, Task> lifecycleMethod)
    {
        foreach (var module in _modules.Values)
        {
            if (module.Instance != null)
            {
                await lifecycleMethod(module.Instance, context);
            }
        }
    }
    

    public IEnumerable<ModuleInfo> GetLoadedModules() => _modules.Values.ToList().AsReadOnly();

    private void PrintModuleHierarchy(ModuleInfo module, int level, HashSet<Type> printedModules)
    {
        if (printedModules.Contains(module.ModuleType)) return;

        printedModules.Add(module.ModuleType);
        Console.WriteLine($"{new string(' ', level * 2)}- {module.ModuleType.Name} created");
        foreach (var dependency in module.Dependencies)
        {
            PrintModuleHierarchy(dependency, level + 1, printedModules);
        }
    }

}
