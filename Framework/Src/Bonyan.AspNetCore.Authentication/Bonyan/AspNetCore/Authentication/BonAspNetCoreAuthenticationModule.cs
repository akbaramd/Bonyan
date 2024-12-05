using Bonyan.AspNetCore.Authorization;
using Bonyan.AspNetCore.Authorization.Permissions;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Bonyan.AspNetCore.Authentication
{
    public class BonAuthenticationModule : BonWebModule
    {
        public BonAuthenticationModule()
        {
            DependOn<BonAspNetCoreModule>();
            
        }

        
        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {

            
            var x = new BonAuthorizationConfiguration(context);

            context.Services.ExecutePreConfiguredActions(x);
            var configure = context.Services.GetPreConfigureActions<AuthorizationOptions>();
            context.Services.AddAuthorization(c =>
            {
                var acc = context.Services.GetObject<PermissionAccessor>();

                foreach (var permission in acc)
                {
                    // Create a policy with the required permissions
                    c.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }

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