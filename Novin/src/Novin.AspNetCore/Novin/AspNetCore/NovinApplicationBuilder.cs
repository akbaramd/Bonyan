using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Novin.AspNetCore.Novin.AspNetCore;

public class NovinApplicationBuilder
{
    internal IBonyanApplicationBuilder BonyanBuilder { get; set; }

    public NovinApplicationBuilder(IBonyanApplicationBuilder bonyanBuilder)
    {
        BonyanBuilder = bonyanBuilder;
    }

    public async Task<NovinApplication> BuildAsync(Action<NovinApplicationContext> action)
    {
        var app = await BonyanBuilder.BuildAsync(ctx =>
        {
            // Enable Swagger in the request pipeline
            ctx.Application.UseSwagger();
            ctx.Application.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); });
            action.Invoke(new NovinApplicationContext(ctx));
        });


        return new NovinApplication(app);
    }
}