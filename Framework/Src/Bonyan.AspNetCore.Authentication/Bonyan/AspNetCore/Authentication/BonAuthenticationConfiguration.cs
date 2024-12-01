using System.Text;
using Bonyan.AspNetCore.Authentication.Options;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.AspNetCore.Authentication
{
    public class BonAuthenticationConfiguration
    {
        private readonly BonConfigurationContext _context;
        private readonly AuthenticationBuilder _authenticationBuilder;
        
        public BonAuthenticationConfiguration(BonConfigurationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authenticationBuilder = context.Services.AddAuthentication();
            UseCookieAuthentication = true; // Default behavior
        }

        public bool UseCookieAuthentication { get; set; } = true;

        /// <summary>
        /// Configures Cookie Authentication.
        /// </summary>
        public void ConfigureCookieAuthentication(Action<BonAuthenticationCookieOptions>? configureOptions = null)
        {
            if (UseCookieAuthentication)
            {
                var options = new BonAuthenticationCookieOptions();
                configureOptions?.Invoke(options);
                _context.Services.AddSingleton(options);
                _authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
                {
                    cookieOptions.LoginPath = options.LoginPath;
                    cookieOptions.AccessDeniedPath = options.AccessDeniedPath;
                    cookieOptions.LogoutPath = options.LogoutPath;
                    cookieOptions.SlidingExpiration = options.SlidingExpiration;
                    cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(options.ExpirationInMinutes);
                    cookieOptions.Cookie.Name = options.CookieName;
                    cookieOptions.Cookie.SecurePolicy = options.CookieSecure
                        ? Microsoft.AspNetCore.Http.CookieSecurePolicy.Always
                        : Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                });
            }
        }

        /// <summary>
        /// Configures JWT Authentication.
        /// </summary>
        public void ConfigureJwtAuthentication(Action<BonAuthenticationJwtOptions>? configureOptions = null)
        {
            var options = new BonAuthenticationJwtOptions();
            configureOptions?.Invoke(options);
            _context.Services.AddSingleton(options);

            if (!options.Enabled || string.IsNullOrWhiteSpace(options.SecretKey))
                throw new InvalidOperationException(
                    "JWT Authentication requires a valid SecretKey and must be enabled.");

            _authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
            {
                jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
                jwtOptions.SaveToken = options.SaveToken;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(options.Issuer),
                    ValidateAudience = !string.IsNullOrEmpty(options.Audience),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience
                };
            });
        }

        /// <summary>
        /// Configures Hybrid Authentication (JWT + Cookie).
        /// </summary>
        public void ConfigureHybridAuthentication(Action<BonAuthenticationJwtOptions>? configureJwt = null,
            Action<BonAuthenticationCookieOptions>? configureCookie = null)
        {
            ConfigureJwtAuthentication(configureJwt);
            ConfigureCookieAuthentication(configureCookie);
        }
    }
}