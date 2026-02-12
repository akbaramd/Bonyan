using Bonyan.AspNetCore.Authentication;
using Bonyan.AspNetCore.Mvc;
using Bonyan.AspNetCore.Swagger;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.WebApi;
using Bonyan.Modularity;
using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BonyanTemplate.WebApi;

/// <summary>
/// Web API host module for the Bonyan Template. Configures HTTP pipeline, Swagger, and health checks.
/// </summary>
public class BonyanTemplateWebApiModule : BonWebModule
{
    /// <inheritdoc />
    public BonyanTemplateWebApiModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonyanTemplateInfrastructureModule>();
        DependOn<BonIdentityManagementWebApiModule>();
        DependOn<BonAspnetCoreSwaggerModule>();
    }

    /// <inheritdoc />
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["ready"]);

        return base.OnConfigureAsync(context, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        context.Application.UseCorrelationId();
        return base.OnApplicationAsync(context, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        context.Application.UseHttpsRedirection();
        context.Application.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
        });
        context.Application.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
        });

        return base.OnPostApplicationAsync(context, cancellationToken);
    }
}
