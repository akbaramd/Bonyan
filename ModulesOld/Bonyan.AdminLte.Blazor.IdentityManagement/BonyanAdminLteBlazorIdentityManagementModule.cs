using Bonyan.AdminLte.Blazor.IdentityManagement.Components.Pages.Account;
using Bonyan.AspNetCore.Components;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Bonyan.AdminLte.Blazor.IdentityManagement;

public class BonyanAdminLteBlazorIdentityManagementModule<TUser> : BonWebModule where TUser : BonIdentityUser
{
    public BonyanAdminLteBlazorIdentityManagementModule()
    {
        DependOn<BonAspNetCoreComponentsModule>();
        DependOn<BonAdminLteBlazorModule>();
        DependOn<BonIdentityManagementApplicationModule<TUser>>();
    }


    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonBlazorOptions>(c =>
        {
            c.AddAssembly<Login>();
        });
        
        
        context.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
                // Additional options
            });

        context.Services.AddAuthorization();
            
        
        return base.OnConfigureAsync(context);
    }
}