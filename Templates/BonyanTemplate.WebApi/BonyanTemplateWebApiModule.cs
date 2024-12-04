using Bonyan.AspNetCore.Authentication;
using Bonyan.AspNetCore.Authorization;
using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Options;
using Bonyan.IdentityManagement.WebApi;
using Bonyan.Modularity;
using BonyanTemplate.Application;
using BonyanTemplate.Domain.Users;
using BonyanTemplate.Infrastructure;
using Microsoft.OpenApi.Models;

namespace BonyanTemplate.WebApi;

public class BonyanTemplateWebApiModule : BonWebModule
{
    public BonyanTemplateWebApiModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
        DependOn<BonIdentityManagementWebApiModule<User>>();
       
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token in the text input below."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        PreConfigure<BonAuthenticationJwtOptions>(c =>
        {
            c.SecretKey = "asdasdasdasdadawdawd453434534534534534534534adw"; // کلید رمزگذاری JWT
            c.Issuer = "your-issuer"; // تنظیم Issuer
            c.Audience = "your-audience"; // تنظیم Audience
            c.Enabled = true; // فعال کردن JWT
            c.RequireHttpsMetadata = true; // نیاز به HTTPS
            c.SaveToken = true; // ذخیره کردن توکن
            c.ExpirationInMinutes = 60; // تنظیم انقضای توکن (در دقیقه)
        });

        PreConfigure<BonAuthorizationConfiguration>(c =>
        {
            c.RegisterPermissions([
                "book.create",
                "book.read",
                "book.delete",
                "book.edit",
            ]);
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
