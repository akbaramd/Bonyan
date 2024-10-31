using Bonyan.Modularity;
using Bonyan.UserManagement.Application;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Web;

public class BonyanUserManagementWebModule<TUser> : WebModule where TUser : BonyanUser
{
    public BonyanUserManagementWebModule()
    {
        DependOn<BonyanUserManagementApplicationModule<TUser>>();
    }

    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }
}