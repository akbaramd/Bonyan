
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Bonyan.AspNetCore.Authentication.Jwt
{
    public class BonAspnetCoreAuthenticationJwtModule : BonWebModule
    {
        public BonAspnetCoreAuthenticationJwtModule()
        {
            DependOn<BonAspnetCoreAuthenticationModule>();
        }

        public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context , CancellationToken cancellationToken = default)
        {
            context.Services.PreConfigure<AuthenticationBuilder>(c =>
            {
                var jwt = new BonAuthenticationJwtOptions();
                context.Services.ExecutePreConfiguredActions(jwt);
                context.Services.AddSingleton(jwt);
                
                if (jwt.Enabled && !string.IsNullOrWhiteSpace(jwt.SecretKey))
                {
                    c.AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = jwt.RequireHttpsMetadata;
                        options.SaveToken = jwt.SaveToken;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = !string.IsNullOrEmpty(jwt.Issuer),
                            ValidateAudience = !string.IsNullOrEmpty(jwt.Audience),
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                            ValidIssuer = jwt.Issuer,
                            ValidAudience = jwt.Audience,
                            ClockSkew = jwt.ClockSkew // Optional: Adjust token expiration tolerance
                        };
                    });
                }
                else
                {
                    throw new InvalidOperationException("JWT Authentication requires a valid SecretKey and must be enabled.");
                }
            });

            return base.OnPreConfigureAsync(context);
        }

        public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            return base.OnPostConfigureAsync(context);
        }

        private void ConfigureAuthentication(BonConfigurationContext context , CancellationToken cancellationToken = default)
        {
            // Additional authentication configuration can be added here if needed
        }

        public override ValueTask OnApplicationAsync(BonWebApplicationContext context,CancellationToken cancellationToken = default)
        {
            return base.OnApplicationAsync(context);
        }
    }
}