﻿using Bonyan.Modularity;
using Bonyan.UserManagement.Application;

namespace Bonyan.UserManagement.Web;


public class BonyanUserManagementWebModule : WebModule
{
    public BonyanUserManagementWebModule()
    {
        DependOn<BonyanUserManagementApplicationModule>();
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