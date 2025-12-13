using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

/// <summary>
/// Options for configuring Unit of Work middleware in ASP.NET Core.
/// </summary>
public class BonyanAspNetCoreUnitOfWorkOptions
{
    /// <summary>
    /// Gets the list of URL patterns to ignore for Unit of Work.
    /// The middleware will be disabled for URLs starting with any of these patterns.
    /// </summary>
    public List<string> IgnoredUrls { get; } = new();
}

/// <summary>
/// Validator for BonyanAspNetCoreUnitOfWorkOptions.
/// </summary>
public class BonyanAspNetCoreUnitOfWorkOptionsValidator : IValidateOptions<BonyanAspNetCoreUnitOfWorkOptions>
{
    public ValidateOptionsResult Validate(string? name, BonyanAspNetCoreUnitOfWorkOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("BonyanAspNetCoreUnitOfWorkOptions cannot be null.");
        }

        if (options.IgnoredUrls == null)
        {
            return ValidateOptionsResult.Fail("IgnoredUrls cannot be null.");
        }

        // Validate URL patterns are not empty strings
        if (options.IgnoredUrls.Any(string.IsNullOrWhiteSpace))
        {
            return ValidateOptionsResult.Fail("IgnoredUrls cannot contain null or empty strings.");
        }

        return ValidateOptionsResult.Success;
    }
}
