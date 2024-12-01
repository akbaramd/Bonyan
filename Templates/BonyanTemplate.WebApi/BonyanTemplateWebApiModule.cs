using Bonyan.AspNetCore.Authentication;
using Bonyan.AspNetCore.Mvc;
using Bonyan.Modularity;
using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;

namespace BonyanTemplate.WebApi;

public class BonyanTemplateWebApiModule : BonWebModule
{
    public BonyanTemplateWebApiModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
        DependOn<BonAspNetCoreMvcModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddSwaggerGen();

        PreConfigure<BonAuthenticationConfiguration>(c =>
        {
            c.ConfigureJwtAuthentication(x =>
            {
                x.Enabled = true;
                x.SecretKey = "AS2Da2s2dK4A5SD8HaAiSD9YAaS2DU285472KHs2d6734haS35";
            });
        });
        return base.OnConfigureAsync(context);
    }


    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseCorrelationId();
        return base.OnApplicationAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        if (context.Application.Environment.IsDevelopment())
        {
            context.Application.UseSwagger();
            context.Application.UseSwaggerUI();
        }

        context.Application.UseHttpsRedirection();


        return base.OnPostApplicationAsync(context);
    }
}