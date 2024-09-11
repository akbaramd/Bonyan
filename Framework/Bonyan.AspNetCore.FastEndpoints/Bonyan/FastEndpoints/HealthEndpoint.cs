using FastEndpoints;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bonyan.FastEndpoints
{
  public class HealthEndpoint : EndpointWithoutRequest
  {
    public override void Configure()
    {
      Get("/bonyan/health");  // Set the route for the health endpoint
      AllowAnonymous(); // Allow anonymous access for health checks
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
      var healthStatus = "Healthy"; // Placeholder for actual health status
      var healthResponse = new
      {
        status = healthStatus,
        uptime = DateTime.UtcNow.Subtract(Environment.TickCount64.MillisecondsToTimeSpan()), // Example uptime
        timestamp = DateTime.UtcNow
      };

      await SendOkAsync(healthResponse, ct); // Return 200 OK with the health information
    }
  }

  // Extension method to convert milliseconds to TimeSpan
  public static class Extensions
  {
    public static TimeSpan MillisecondsToTimeSpan(this long milliseconds)
    {
      return TimeSpan.FromMilliseconds(milliseconds);
    }
  }
}
