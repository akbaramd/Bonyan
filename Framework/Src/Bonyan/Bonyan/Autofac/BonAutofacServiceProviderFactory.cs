using System;
using System.IO;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bonyan.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Volo.Abp.Autofac;

/// <summary>
/// A factory for creating a <see cref="T:ContainerBuilder" /> and an <see cref="T:System.IServiceProvider" />.
/// </summary>
public class AbpAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    private const string AgentLogPath = @"c:\Users\ahmadi.UR-NEZAM\RiderProjects\Bonyan\.cursor\debug.log";

    private readonly ContainerBuilder _builder;
    private IServiceCollection _services = default!;

    public AbpAutofacServiceProviderFactory(ContainerBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="services">The collection of services</param>
    /// <returns>A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.</returns>
    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        _services = services;

#region agent log
        try
        {
            try
            {
                Console.Error.WriteLine($"[agent-log] AbpAutofacServiceProviderFactory.CreateBuilder reached | asm={typeof(AbpAutofacServiceProviderFactory).Assembly.Location} | servicesCount={services.Count}");
            }
            catch
            {
                // ignore
            }

            var dir = Path.GetDirectoryName(AgentLogPath);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.AppendAllText(
                AgentLogPath,
                JsonSerializer.Serialize(new
                {
                    id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    runId = "pre-fix",
                    hypothesisId = "H4",
                    location = "BonAutofacServiceProviderFactory.cs:CreateBuilder",
                    message = "CreateBuilder called",
                    data = new
                    {
                        servicesCount = services.Count,
                        asm = typeof(AbpAutofacServiceProviderFactory).Assembly.Location
                    }
                }) + Environment.NewLine);
        }
        catch (Exception ex)
        {
            try
            {
                Console.Error.WriteLine($"[agent-log-write-failed] BonAutofacServiceProviderFactory.cs:CreateBuilder :: {ex.GetType().FullName} :: {ex.Message}");
            }
            catch
            {
                // ignore
            }
        }
#endregion

        _builder.Populate(services);

        return _builder;
    }

    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        Check.NotNull(containerBuilder, nameof(containerBuilder));

#region agent log
        try
        {
            try
            {
                Console.Error.WriteLine($"[agent-log] AbpAutofacServiceProviderFactory.CreateServiceProvider reached | asm={typeof(AbpAutofacServiceProviderFactory).Assembly.Location}");
            }
            catch
            {
                // ignore
            }

            var dir = Path.GetDirectoryName(AgentLogPath);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.AppendAllText(
                AgentLogPath,
                JsonSerializer.Serialize(new
                {
                    id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    runId = "pre-fix",
                    hypothesisId = "H4",
                    location = "BonAutofacServiceProviderFactory.cs:CreateServiceProvider",
                    message = "CreateServiceProvider called (about to Build container)",
                    data = new
                    {
                        asm = typeof(AbpAutofacServiceProviderFactory).Assembly.Location
                    }
                }) + Environment.NewLine);
        }
        catch (Exception ex)
        {
            try
            {
                Console.Error.WriteLine($"[agent-log-write-failed] BonAutofacServiceProviderFactory.cs:CreateServiceProvider :: {ex.GetType().FullName} :: {ex.Message}");
            }
            catch
            {
                // ignore
            }
        }
#endregion

        return new AutofacServiceProvider(containerBuilder.Build());
    }
}