using Bonyan.Modularity;
using Microsoft.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc.Localization;

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
                endpointContext.Endpoints.MapControllerRoute("defaultWithArea",
                    "{area}/{controller=Home}/{action=Index}/{id?}");
                endpointContext.Endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpointContext.Endpoints.MapRazorPages();
            });
        });
        return base.OnConfigureAsync(context);
    }
}