namespace Bonyan.Modularity;

public interface IWebModularityApplication : IModularityApplication
{
  Task ApplicationAsync(ModularityApplicationContext bonyanApplication);
}
