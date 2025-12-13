using Bonyan.AspNetCore;
using Bonyan.AspNetCore.Mvc;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace WebApi.Demo.Modules;

/// <summary>
/// Root module for the Web API Demo application.
/// Handles both service configuration and web application pipeline setup.
/// </summary>
public class WebApiDemoModule : BonWebModule
{
    // Dependencies are declared using constructor-based style: DependOn<TModule>()
    
    public WebApiDemoModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<ProductModule>();
    }

    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Early configuration before other modules configure
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Register application services
        context.Services.AddControllers();
        context.Services.AddEndpointsApiExplorer();
        
        // Configure Swagger
        context.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Web API Demo",
                Version = "1.0.0",
                Description = "A demonstration of Bonyan modular web API",
                Contact = new OpenApiContact
                {
                    Name = "Bonyan Framework",
                    Email = "support@bonyan.io"
                }
            });
        });

        // Configure application options
        context.ConfigureOptions<WebApiDemoOptions>(options =>
        {
            options.ApplicationName = "Web API Demo";
            options.Version = "1.0.0";
            options.EnableSwagger = true;
        });

        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Final configuration after all modules have configured
        return base.OnPostConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnPreInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Early initialization
        return base.OnPreInitializeAsync(context, cancellationToken);
    }

    public override ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Main initialization
        return base.OnInitializeAsync(context, cancellationToken);
    }

    public override ValueTask OnPostInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Final initialization
        return base.OnPostInitializeAsync(context, cancellationToken);
    }

    /// <summary>
    /// Configures the web application pipeline before middleware setup.
    /// Use this phase for static files, CORS, etc.
    /// </summary>
    public override ValueTask OnPreApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Static files are already configured by BonAspNetCoreModule
        // Add any additional pre-middleware configuration here if needed

        return base.OnPreApplicationAsync(context, cancellationToken);
    }

    /// <summary>
    /// Configures the web application middleware pipeline.
    /// Use this phase for routing, authentication, authorization, etc.
    /// </summary>
    public override ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        var app = context.Application;

        // Configure Swagger UI (only in Development)
        if (context.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API Demo V1");
                options.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
            });
        }

        // Map controllers
        app.MapControllers();

        return base.OnApplicationAsync(context, cancellationToken);
    }

    /// <summary>
    /// Configures the web application after middleware setup.
    /// Use this phase for endpoint configuration, health checks, etc.
    /// </summary>
    public override ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Additional endpoint configuration can be added here
        // For example, health checks, custom endpoints, etc.

        return base.OnPostApplicationAsync(context, cancellationToken);
    }
}

/// <summary>
/// Configuration options for the Web API Demo application.
/// </summary>
public class WebApiDemoOptions
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool EnableSwagger { get; set; } = true;
}

