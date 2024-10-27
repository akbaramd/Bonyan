using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;

namespace Bonyan.FastEndpoints.Security
{
  [DependOn(typeof(BonyanFastEndpointModule))]
  public class BonyanFastEndpointSecurityModule : Module
  {
    private const int MinimumSigningKeyLength = 16; // Minimum 16 characters (128 bits) for HS256

    public override Task OnConfigureAsync(ModularityContext context)
    {
      // Attempt to retrieve JwtSigningOptions
      var jwtSigningOptions = context.RequiredOption<JwtSigningOptions>();

      // Validate the size of the SigningKey
      if (jwtSigningOptions?.SigningKey?.Length < MinimumSigningKeyLength)
      {
        throw new InvalidOperationException(
          $"SigningKey in JwtSigningOptions is too short. It must be at least {MinimumSigningKeyLength} characters long. Please configure it using context.Services.Configure<JwtSigningOptions>() in your main module. {nameof(IModule.OnPreConfigureAsync)}");
      }

      // Add JWT Bearer Authentication using the configured options via a lambda
      context.Services.AddAuthenticationJwtBearer(options =>
      {
        options.SigningKey = jwtSigningOptions?.SigningKey;
        // Configure other properties as needed
      });

      context.Services.AddAuthorization();


      context.Services.Configure<FastEndpointConfiguration>(c =>
      {
        c.AddBeforeInitializer(app =>
        {
          app.UseAuthentication();
          app.UseAuthorization();
        });
      });
      
      return base.OnConfigureAsync(context);
    }
  }
}
