using Bonyan.AspNetCore.Authentication;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds and configures authentication for the application.
        /// </summary>
        public static BonConfigurationContext AddAuthentication(
            this BonConfigurationContext context,
            Action<BonAuthenticationConfiguration> configure)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var manager = new BonAuthenticationConfiguration(context);
            manager.ConfigureCookieAuthentication();
            configure(manager);


            return context;
        }

        public static BonConfigurationContext AddAuthorization(
            this BonConfigurationContext context,
            Action<BonAuthorizationConfiguration> configure)
        {
            var v = new BonAuthorizationConfiguration(context);
            configure(v);

            context.Services.AddAuthorization(c =>
            {
                var acc = context.Services.GetObject<PermissionAccessor>();
                
                foreach (var permission in acc)
                {
                    // Create a policy with the required permissions
                    c.AddPolicy(permission, policy =>
                        policy.RequireClaim(permission));
                }
            });

            return context;
        }
    }
}