using Microsoft.AspNetCore.Http;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Serializes exception response models to HTTP response.
/// Part of the microkernel architecture - allows pluggable serialization strategies.
/// </summary>
public interface IExceptionResponseSerializer
{
    /// <summary>
    /// Serializes the response model and writes it to the HTTP response.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="responseModel">The response model to serialize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SerializeAsync(HttpContext httpContext, object responseModel, CancellationToken cancellationToken = default);
}

