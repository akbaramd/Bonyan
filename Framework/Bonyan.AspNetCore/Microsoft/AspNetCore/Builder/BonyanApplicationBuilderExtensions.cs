using Bonyan.AspNetCore.Security;
using Bonyan.ExceptionHandling;
using Bonyan.UnitOfWork;


namespace Microsoft.AspNetCore.Builder;

public static class BonyanApplicationBuilderExtensions
{
    private const string ExceptionHandlingMiddlewareMarker = "_BonyanExceptionHandlingMiddleware_Added";

   

    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<BonyanUnitOfWorkMiddleware>();
    }

   

  

    public static IApplicationBuilder UseBonyanExceptionHandling(this IApplicationBuilder app)
    {
        if (app.Properties.ContainsKey(ExceptionHandlingMiddlewareMarker))
        {
            return app;
        }

        app.Properties[ExceptionHandlingMiddlewareMarker] = true;
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseBonyanClaimsMap(this IApplicationBuilder app)
    {
        return app.UseMiddleware<BonyanClaimsMapMiddleware>();
    }

  
}
