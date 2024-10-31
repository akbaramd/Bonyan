using Bonyan.Modularity;
using Bonyan.UserManagement.Application;
using Bonyan.UserManagement.Domain;

namespace Bonyan.IdentityManagement.Web;

public class BonyanIdentityManagementWebModule<TUser> : WebModule where TUser : BonyanUser
{
    public BonyanIdentityManagementWebModule()
    {
        DependOn<BonyanUserManagementApplicationModule<TUser>>();
    }

}