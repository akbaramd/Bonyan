namespace Microsoft.AspNetCore.Builder;

public interface IWebApplicationInitializer
{
  Task InitializeAsync(WebApplication application);
}
