using System.Runtime.Loader;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Plugins;

public class FilePlugInSource : IPlugInSource
{
    public string[] FilePaths { get; }

    public FilePlugInSource(params string[]? filePaths)
    {
        FilePaths = filePaths ?? new string[0];
    }

    public Type[] GetModules()
    {
        var modules = new List<Type>();

        foreach (var filePath in FilePaths)
        {
            var absolutePath = Path.GetFullPath(filePath); // Convert to absolute path
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(absolutePath);

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
                throw new BonException("Could not get module types from assembly: " + assembly.FullName, ex);
            }
        }

        return modules.ToArray();
    }
}
