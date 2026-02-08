using Bonyan.Modularity;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bonyan.AspNetCore.Swagger
{
    public class BonAspnetCoreSwaggerModule : BonWebModule
    {
        public BonAspnetCoreSwaggerModule()
        {
            DependOn<BonAspNetCoreModule>();
        }


        
        public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            var swaggerGenPreOptions = context.Services.GetPreConfigureActions<SwaggerGenOptions>();
            context.Services.AddSwaggerGen(options =>
            {
                swaggerGenPreOptions.Configure(options);
            });
            return base.OnPostConfigureAsync(context);
        }

        public override ValueTask OnApplicationAsync(BonWebApplicationContext context , CancellationToken cancellationToken = default)
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