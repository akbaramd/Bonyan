using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Default implementation using System.Text.Json for exception response serialization.
/// </summary>
public class SystemTextJsonExceptionResponseSerializer : IExceptionResponseSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the response model using System.Text.Json and writes it to the HTTP response.
    /// </summary>
    public async Task SerializeAsync(HttpContext httpContext, object responseModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(responseModel);

        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(responseModel, DefaultOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        await httpContext.Response.Body.WriteAsync(bytes, cancellationToken);
    }
}

