namespace Bonyan.Modularity.Abstractions;

public interface IWebModularityApplication : IModularityApplication
{
  Task InitializeApplicationAsync(WebApplication application);
}
