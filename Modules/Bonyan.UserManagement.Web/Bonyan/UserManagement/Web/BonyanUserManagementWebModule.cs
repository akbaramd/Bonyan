using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.UserManagement.Application;

namespace Bonyan.UserManagement.Web;

[DependOn([
    typeof(BonyanUserManagementApplicationModule),
])]
public class BonyanUserManagementWebModule : WebModule
{
    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {

        return base.OnConfigureAsync(context);
    }
}