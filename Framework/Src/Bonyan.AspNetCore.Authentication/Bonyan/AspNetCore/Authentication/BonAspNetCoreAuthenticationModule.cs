
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

        
        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            
            var configure = context.Services.GetPreConfigureActions<AuthorizationOptions>();
            
            context.Services.AddAuthorization(c =>
            {
                configure.Configure(c);
            });


            ConfigureAuthentication(context);
            return base.OnPostConfigureAsync(context);
        }

        private void ConfigureAuthentication(BonConfigurationContext context)
        {
            var options = GetPreConfigure<AuthenticationOptions>();
            var bonPreConfigureAction = GetPreConfigure<AuthenticationBuilder>();
            var builder = context.Services.AddAuthentication(manager => { options.Configure(manager); });
            bonPreConfigureAction.Configure(builder);
            
            
        }

        public override Task OnApplicationAsync(BonWebApplicationContext context)
        {
            context.Application.UseAuthentication();
            context.Application.UseAuthorization();
            return base.OnApplicationAsync(context);
        }
    }
}