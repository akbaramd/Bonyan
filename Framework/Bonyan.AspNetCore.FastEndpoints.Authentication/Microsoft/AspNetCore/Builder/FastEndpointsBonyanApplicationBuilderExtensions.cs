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
    configuration.Builder.GetServicesCollection()
      .AddAuthenticationJwtBearer(jwtSigningOptions);

    configuration.Builder.GetServicesCollection().AddAuthentication();
    configuration.Builder.GetServicesCollection().AddAuthorization();

    configuration.Builder.AddBeforeInitializer(app =>
    {
      app.Application.UseAuthentication() //add this
        .UseAuthorization();
    });
    
    return configuration;
  }
}
