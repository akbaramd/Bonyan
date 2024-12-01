using System.Reflection;
using System.Runtime.Loader;
using Bonyan.Core;
using Bonyan.Modularity.Abstractions;
using Bonyan.Reflection;
using JetBrains.Annotations;

namespace Bonyan.Plugins;

public class FolderPlugInSource : IPlugInSource
{
    public string Folder { get; }

    public SearchOption SearchOption { get; set; }

    public Func<string, bool>? Filter { get; set; }

    public FolderPlugInSource(
        [NotNull] string folder,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        Check.NotNull(folder, nameof(folder));

        Folder = folder;
        SearchOption = searchOption;
    }

    public Type[] GetModules()
    {
        var modules = new List<Type>();

        foreach (var assembly in GetAssemblies())
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (BonModule.IsBonyanModule(type))
                    {
                        modules.AddIfNotContains(type);
                    }
                }
            }
            catch (Exception ex)
            {
                // throw new BonException("Could not get module types from assembly: " + assembly.FullName, ex);
            }
        }

        return modules.ToArray();
    }

    private List<Assembly> GetAssemblies()
    {
        var assemblyFiles = AssemblyHelper.GetAssemblyFiles(Folder, SearchOption);

        if (Filter != null)
        {
            assemblyFiles = assemblyFiles.Where(Filter);
        }

        return assemblyFiles.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath).ToList();
    }
}
