using Bonyan.IdentityManagement.Domain.Permissions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.AspNetCore.Authorization.Permissions;

public class BonPermissionManager : IBonPermissionManager
{
    private readonly IServiceProvider _serviceProvider;

    public BonPermissionManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<BonIdentityPermission> GetAllPermissions()
    {
        var providers = _serviceProvider.GetServices<IBonPermissionProvider>();

        return providers.SelectMany(x => x.GetPermissions());
    }
}