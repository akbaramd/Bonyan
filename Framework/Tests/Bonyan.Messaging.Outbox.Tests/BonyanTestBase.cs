using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.Outbox.Tests;

/// <summary>
/// Base class for Bonyan application tests that provides proper application lifecycle management using BonyanApplicationBuilder.
/// </summary>
/// <typeparam name="TModule">The root module type for the test application.</typeparam>
public abstract class BonyanTestBase<TModule> : IDisposable where TModule : IBonModule
{
    protected WebApplication Application { get; private set; } = null!;
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected IHostedService? HostedService { get; private set; }
    private bool _disposed = false;

    /// <summary>
    /// Initializes the Bonyan application for testing using BonyanApplicationBuilder.
    /// </summary>
    /// <param name="serviceName">The name of the service for testing.</param>
    /// <param name="configureServices">Optional action to configure additional services.</param>
    /// <param name="configureApplication">Optional action to configure the application after building.</param>
    protected async Task InitializeApplicationAsync(
        string serviceName = "TestService",
        Action<BonConfigurationContext>? configureServices = null,
        Action<BonWebApplicationContext>? configureApplication = null)
    {
        // Create the Bonyan application builder directly
        var builder = BonyanApplication.CreateModularBuilder<TModule>(
            serviceName,
            configureServices);

        // Configure logging for tests
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        // Build the application using BonyanApplicationBuilder
        Application = await builder.BuildAsync(configureApplication);
        ServiceProvider = Application.Services;

        // Start hosted services if any
        await StartHostedServicesAsync();
    }

    /// <summary>
    /// Starts all hosted services in the application.
    /// </summary>
    private async Task StartHostedServicesAsync()
    {
        var hostedServices = ServiceProvider.GetServices<IHostedService>();
        
        foreach (var hostedService in hostedServices)
        {
            HostedService = hostedService;
            await hostedService.StartAsync(CancellationToken.None);
        }
    }

    /// <summary>
    /// Stops all hosted services in the application.
    /// </summary>
    private async Task StopHostedServicesAsync()
    {
        if (HostedService != null)
        {
            await HostedService.StopAsync(CancellationToken.None);
            HostedService = null;
        }
    }

    /// <summary>
    /// Gets a service from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The requested service instance.</returns>
    protected T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider, or null if not registered.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The requested service instance or null.</returns>
    protected T? GetOptionalService<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Disposes the test application and all its resources.
    /// </summary>
    public virtual void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                StopHostedServicesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log the exception but don't throw during disposal
                Console.WriteLine($"Error stopping hosted services: {ex.Message}");
            }

            try
            {
                Application?.StopAsync();
            }
            catch (Exception ex)
            {
                // Log the exception but don't throw during disposal
                Console.WriteLine($"Error disposing application: {ex.Message}");
            }

         

            _disposed = true;
        }
    }
}
