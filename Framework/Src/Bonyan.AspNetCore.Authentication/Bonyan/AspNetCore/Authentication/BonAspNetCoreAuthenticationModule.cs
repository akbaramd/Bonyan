
using Bonyan.Modularity;
using Bonyan.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Bonyan.AspNetCore.Authentication
{
    public class BonAspnetCoreAuthenticationModule : BonWebModule
    {
        public BonAspnetCoreAuthenticationModule()
        {
            
            DependOn<BonSecurityModule>();
            DependOn<BonAspNetCoreModule>();
            
        }

        
        public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            
            var configure = context.Services.GetPreConfigureActions<AuthorizationOptions>();
            
            context.Services.AddAuthorization(c =>
            {
                configure.Configure(c);
            });


            ConfigureAuthentication(context);
            return base.OnPostConfigureAsync(context);
        }

        private void ConfigureAuthentication(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            var options = GetPreConfigure<AuthenticationOptions>();
            var bonPreConfigureAction = GetPreConfigure<AuthenticationBuilder>();
            var builder = context.Services.AddAuthentication(manager => { options.Configure(manager); });
            bonPreConfigureAction.Configure(builder);
            
            
        }

        public override ValueTask OnApplicationAsync(BonWebApplicationContext context,CancellationToken  cancellationToken = default)
        {
            context.Application.UseAuthentication();
            context.Application.UseAuthorization();
            return base.OnApplicationAsync(context);
        }
    }
}