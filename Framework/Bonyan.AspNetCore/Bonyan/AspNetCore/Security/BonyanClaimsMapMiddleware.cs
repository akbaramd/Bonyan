using System.Security.Claims;
using Bonyan.Security.Claims;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Security;

public class BonyanClaimsMapMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var currentPrincipalAccessor = context.RequestServices
            .GetRequiredService<ICurrentPrincipalAccessor>();

        var mapOptions = context.RequestServices
            .GetRequiredService<IOptions<BonyanClaimsMapOptions>>().Value;

        var mapClaims = currentPrincipalAccessor
            .Principal
            .Claims
            .Where(claim => mapOptions.Maps.Keys.Contains(claim.Type));

        currentPrincipalAccessor
            .Principal
            .AddIdentity(
                new ClaimsIdentity(
                    mapClaims
                        .Select(
                            claim => new Claim(
                                mapOptions.Maps[claim.Type](),
                                claim.Value,
                                claim.ValueType,
                                claim.Issuer
                            )
                        )
                )
            );

        await next(context);
    }
}
