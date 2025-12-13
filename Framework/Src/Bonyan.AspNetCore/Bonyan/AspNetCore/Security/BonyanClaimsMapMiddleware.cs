using System.Security.Claims;
using Bonyan.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Security;

/// <summary>
/// Middleware that maps claims from one type to another based on configuration.
/// Part of the microkernel architecture - provides pluggable claims transformation.
/// </summary>
public class BonyanClaimsMapMiddleware : IMiddleware
{
    private readonly BonyanClaimsMapOptions _options;
    private readonly ILogger<BonyanClaimsMapMiddleware>? _logger;

    public BonyanClaimsMapMiddleware(
        IOptions<BonyanClaimsMapOptions> options,
        ILogger<BonyanClaimsMapMiddleware>? logger = null)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            var currentPrincipalAccessor = context.RequestServices
                .GetRequiredService<IBonCurrentPrincipalAccessor>();

            if (currentPrincipalAccessor?.Principal == null)
            {
                _logger?.LogDebug("No principal found, skipping claims mapping");
                await next(context);
                return;
            }

            var mapClaims = currentPrincipalAccessor.Principal.Claims
                .Where(claim => _options.Maps.ContainsKey(claim.Type))
                .ToList();

            if (mapClaims.Any())
            {
                var mappedClaims = mapClaims
                    .Select(claim =>
                    {
                        var targetClaimType = _options.Maps[claim.Type]();
                        return new Claim(
                            targetClaimType,
                            claim.Value,
                            claim.ValueType,
                            claim.Issuer,
                            claim.OriginalIssuer);
                    })
                    .ToList();

                currentPrincipalAccessor.Principal.AddIdentity(new ClaimsIdentity(mappedClaims));
                
                _logger?.LogDebug("Mapped {ClaimCount} claims for principal", mappedClaims.Count);
            }

            await next(context);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in claims mapping middleware");
            throw;
        }
    }
}
