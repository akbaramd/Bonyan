﻿using Bonyan.Modularity;

namespace Bonyan.AspNetCore.Mvc;

public class BonAspNetCoreMvcModule : BonWebModule
{
    public BonAspNetCoreMvcModule()
    {
        DependOn<BonAspNetCoreModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        var builder = context.Services.AddMvc();
        builder.AddControllersAsServices();
        builder.AddViewComponentsAsServices();

        context.Services.ExecutePreConfiguredActions(builder);

        Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapControllerRoute("defaultWithArea",
                    "{area}/{controller=Home}/{action=Index}/{id?}");
                endpointContext.Endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpointContext.Endpoints.MapRazorPages();
            });
        });
        return base.OnConfigureAsync(context);
    }
}