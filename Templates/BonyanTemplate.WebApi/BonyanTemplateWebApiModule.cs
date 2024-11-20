using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.EntityFrameworkCore;
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

        context.Services.Configure<BonJwtOptions>(c =>
        {
            c.Enabled = true;
            c.SecretKey = "sss";
        });
        
        return base.OnConfigureAsync(context);
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