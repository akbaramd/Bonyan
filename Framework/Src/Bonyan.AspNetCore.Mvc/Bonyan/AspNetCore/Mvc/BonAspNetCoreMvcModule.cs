using Bonyan.AspNetCore.Mvc.Localization;
using Bonyan.Modularity;
using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Mvc;




public class BonAspNetCoreMvcModule : BonWebModule
{
    public BonAspNetCoreMvcModule()
    {
        DependOn<BonAspNetCoreModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        
        var abpMvcDataAnnotationsLocalizationOptions = context.Services
            .ExecutePreConfiguredActions(
                new BonMvcDataAnnotationsLocalizationOptions()
            );
        
        var builder = context.Services.AddMvc()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var resourceType = abpMvcDataAnnotationsLocalizationOptions
                        .AssemblyResources
                        .GetOrDefault(type.Assembly);

                    if (resourceType != null)
                    {
                        return factory.Create(resourceType);
                    }

                    return factory.CreateDefaultOrNull() ??
                           factory.Create(type);
                };
            })
            .AddViewLocalization();
        
        builder.AddControllersAsServices();
        builder.AddViewComponentsAsServices();

        context.Services.ExecutePreConfiguredActions(builder);

        Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                // 1️⃣ Default route first
                endpointContext.Endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                // 2️⃣ Area route only after default
                endpointContext.Endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpointContext.Endpoints.MapRazorPages();
            });
        });
        return base.OnConfigureAsync(context);
    }
}