using System.Reflection;

namespace Bonyan.Reflection;

public interface IAssemblyFinder
{
    IReadOnlyList<Assembly> Assemblies { get; }
}
