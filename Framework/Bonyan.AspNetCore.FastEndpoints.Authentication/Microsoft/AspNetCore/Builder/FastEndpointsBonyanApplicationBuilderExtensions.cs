using Bonyan.FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.Builder;

public static class FastEndpointsBonyanApplicationBuilderExtensions
{
  public static FastEndpointConfiguration AddAuthentication(this FastEndpointConfiguration configuration,
    Action<JwtSigningOptions> jwtSigningOptions)
  {
    configuration.Services
      .AddAuthenticationJwtBearer(jwtSigningOptions);

    configuration.Services.AddAuthentication();
    configuration.Services.AddAuthorization();
    return configuration;
  }
}
