using Bonyan.Modularity;
using Bonyan.UserManagement.Application;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Web;

public class BonUserManagementWebModule<TUser> : WebModule where TUser : BonUser
{
    public BonUserManagementWebModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }
}