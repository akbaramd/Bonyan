using Bonyan.ExceptionHandling;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.Options;

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
        context.Services.AddAuthorization();
        context.Services.AddHttpContextAccessor();
        
        return base.OnConfigureAsync(context);
    }

    public override Task OnPreApplicationAsync(ApplicationContext context)
    {
        var exceptionHandlingOptions = context.GetService<IOptions<ExceptionHandlingOptions>>()?.Value;
        if (exceptionHandlingOptions is { ApiExceptionMiddlewareEnabled: true })
        {
            context.Application.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        return base.OnPreApplicationAsync(context);
    }
}