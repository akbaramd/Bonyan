using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public class ApplicationContext : ServiceContextBase
{
    public WebApplication Application { get; }

    public ApplicationContext(WebApplication application)
        : base(application.Services)
    {
        Application = application;
    }
}