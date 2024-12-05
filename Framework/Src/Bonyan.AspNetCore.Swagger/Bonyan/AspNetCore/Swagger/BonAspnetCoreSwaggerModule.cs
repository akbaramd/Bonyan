using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bonyan.AspNetCore.Authentication
{
    public class BonAspnetCoreSwaggerModule : BonWebModule
    {
        public BonAspnetCoreSwaggerModule()
        {
            DependOn<BonAspNetCoreModule>();
        }


        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            var swaggerGenPreOptions = context.Services.GetPreConfigureActions<SwaggerGenOptions>();
            context.Services.AddSwaggerGen(options =>
            {
                swaggerGenPreOptions.Configure(options);
            });
            return base.OnPostConfigureAsync(context);
        }

        public override Task OnApplicationAsync(BonWebApplicationContext context)
        {
            if (context.Application.Environment.IsDevelopment())
            {
                context.Application.UseSwagger();
                context.Application.UseSwaggerUI();
            }
            
            return base.OnApplicationAsync(context);
        }
    }
}