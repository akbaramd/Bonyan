using System;
using Bonyan.AspNetCore.Authentication;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

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
    }
}