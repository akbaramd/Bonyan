using Bonyan.MultiTenant;
using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Application.Services;

public class ApplicationService : IApplicationService
{
    public ApplicationService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }
    public ICurrentUser CurrentUser => ServiceProvider.GetRequiredService<ICurrentUser>();
    public ICurrentTenant CurrentTenant  => ServiceProvider.GetRequiredService<ICurrentTenant>();
}