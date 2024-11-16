using System.Reflection;

namespace Bonyan.Modularity;

public interface IAdditionalModuleAssemblyProvider
{
  Assembly[] GetAssemblies();
}