using Microsoft.AspNetCore.Http;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Maps exceptions to HTTP results with appropriate status codes and response models.
/// Part of the microkernel architecture - allows pluggable exception handling strategies.
/// </summary>
public interface IExceptionToHttpResultMapper
{
    /// <summary>
    /// Maps an exception to an HTTP result.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The HTTP result model with status code and response body.</returns>
    ExceptionHttpResult MapToHttpResult(Exception exception, HttpContext httpContext);
}

/// <summary>
/// Represents the HTTP result of an exception mapping.
/// </summary>
public class ExceptionHttpResult
{
    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Gets the response body model.
    /// </summary>
    public object ResponseBody { get; init; } = null!;
}

