using Bonyan.AspNetCore.Security;
using Bonyan.ExceptionHandling;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.UnitOfWork;
using Microsoft;

namespace Bonyan.AspNetCore;

[DependOn(typeof(BonyanLayerDomainModule))]
public class BonyanAspNetCoreModule : WebModule
{
    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        context.Configure<ExceptionHandlingOptions>(c => c.ApiExceptionMiddlewareEnabled = false);
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {

        context.Services.AddTransient<BonyanClaimsMapMiddleware>();
        context.Services.AddTransient<BonyanUnitOfWorkMiddleware>();
        
        context.Services.AddAuthorization();
        context.Services.AddHttpContextAccessor();
        
        
        context.Services.AddObjectAccessor<IApplicationBuilder>();
        
        return base.OnConfigureAsync(context);
    }

    public override Task OnApplicationAsync(ApplicationContext context)
    {
        context.Application.UseAuthorization();
        return base.OnApplicationAsync(context);
    }
}