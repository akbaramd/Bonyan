using Microsoft.Extensions.Options;

namespace Bonyan.ExceptionHandling;

/// <summary>
/// Options for configuring exception handling middleware.
/// </summary>
public class ExceptionHandlingOptions
{
    /// <summary>
    /// Gets or sets whether the API exception middleware is enabled.
    /// </summary>
    public bool ApiExceptionMiddlewareEnabled { get; set; } = false;
}

/// <summary>
/// Validator for ExceptionHandlingOptions.
/// </summary>
public class ExceptionHandlingOptionsValidator : IValidateOptions<ExceptionHandlingOptions>
{
    public ValidateOptionsResult Validate(string? name, ExceptionHandlingOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("ExceptionHandlingOptions cannot be null.");
        }

        return ValidateOptionsResult.Success;
    }
}