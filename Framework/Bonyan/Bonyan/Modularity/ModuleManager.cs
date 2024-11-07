using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity
{
    public class ModuleManager : IModuleManager
    {
        private readonly IBonModuleAccessor _bonModuleAccessor;

        public ModuleManager(IBonModuleAccessor bonModuleAccessor)
        {
            _bonModuleAccessor = bonModuleAccessor;
        }

        public void LoadModules(Type mainModuleType)
        {
            ValidateMainModuleType(mainModuleType);
            LoadModuleRecursive(mainModuleType);

            var sortedModules = TopologicalSort(_bonModuleAccessor.GetAllModules().ToList());
            UpdateModuleListAndInstantiate(sortedModules);
        }

        private void ValidateMainModuleType(Type mainModuleType)
        {
            if (mainModuleType == null)
                throw new ArgumentNullException(nameof(mainModuleType));

            if (!typeof(IBonModule).IsAssignableFrom(mainModuleType))
                throw new ArgumentException($"Type {mainModuleType.FullName} does not implement IModule.");
        }

        private void LoadModuleRecursive(Type moduleType)
        {
            if (_bonModuleAccessor.GetModule(moduleType) != null) return;

            var moduleInstance = (IBonModule)Activator.CreateInstance(moduleType)!;
            var moduleInfo = new ModuleInfo(moduleType) { Instance = moduleInstance };
            _bonModuleAccessor.AddModule(moduleInfo);

            foreach (var dependencyType in moduleInstance.DependedModules)
            {
                LoadModuleRecursive(dependencyType);
                moduleInfo.Dependencies.Add(_bonModuleAccessor.GetModule(dependencyType)!);
            }
        }

        private List<ModuleInfo> TopologicalSort(List<ModuleInfo> modules)
        {
            var sorted = new List<ModuleInfo>();
            var visited = new Dictionary<ModuleInfo, bool>();

            foreach (var module in modules)
            {
                if (!Visit(module, visited, sorted))
                    throw new InvalidOperationException($"Circular dependency detected in {module.ModuleType.FullName}.");
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

        private void UpdateModuleListAndInstantiate(IEnumerable<ModuleInfo> sortedModules)
        {
            _bonModuleAccessor.ClearModules();
            foreach (var module in sortedModules)
            {
                module.Instance ??= (IBonModule)Activator.CreateInstance(module.ModuleType)!;
                _bonModuleAccessor.AddModule(module);
            }
        }

        public static void ValidateModuleType(Type moduleType)
        {
            if (!typeof(IBonModule).IsAssignableFrom(moduleType) || moduleType.IsAbstract || moduleType.IsGenericType)
            {
                throw new ArgumentException($"Type {moduleType.FullName} is not a valid module.");
            }
        }
    }
}
