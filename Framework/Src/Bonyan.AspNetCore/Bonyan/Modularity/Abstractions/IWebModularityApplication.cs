namespace Bonyan.Modularity.Abstractions;

public interface IWebBonModularityApplication : IBonModularityApplication
{
  Task InitializeApplicationAsync(WebApplication application);
}