using System;
using System.Threading.Tasks;
using Bonyan.AspNetCore.Authentication;
using Bonyan.AspNetCore.Authentication.Options;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

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
            ConfigureAuthentication(context);
            return base.OnPostConfigureAsync(context);
        }

        private void ConfigureAuthentication(BonConfigurationContext context)
        {
            var options = GetPreConfigure<BonAuthenticationConfiguration>();
            context.AddAuthentication(manager => { options.Configure(manager); });
        }

        public override Task OnApplicationAsync(BonWebApplicationContext context)
        {
            context.Application.UseAuthentication();
            context.Application.UseAuthorization();
            return base.OnApplicationAsync(context);
        }
    }
}