using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Options;
using WebApi.Demo.Modules;

namespace WebApi.Demo.Modules;

/// <summary>
/// Swagger/OpenAPI module for API documentation.
/// </summary>
[DependsOn(typeof(WebApiDemoModule))]
public class SwaggerModule : BonModule
{
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        var options = context.GetOption<WebApiDemoOptions>();
        
        if (options?.EnableSwagger == true)
        {
            context.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Web API Demo",
                    Version = "v1",
                    Description = "A demo Web API using Bonyan Modularity System"
                });
            });
        }

        return ValueTask.CompletedTask;
    }
}

