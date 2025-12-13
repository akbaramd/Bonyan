namespace Bonyan.Modularity;

/// <summary>
/// Context for managing application services and environment settings during web application initialization.
/// Part of the microkernel architecture - provides controlled access to web application features.
/// </summary>
public class BonWebApplicationContext : BonContextBase
{
    /// <summary>
    /// Gets the web application instance.
    /// </summary>
    public WebApplication Application { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BonWebApplicationContext"/>.
    /// </summary>
    /// <param name="application">The web application instance.</param>
    /// <exception cref="ArgumentNullException">Thrown if application is null.</exception>
    public BonWebApplicationContext(WebApplication application)
        : base(application?.Services ?? throw new ArgumentNullException(nameof(application)))
    {
        Application = application;
    }

    /// <summary>
    /// Registers middleware in the application pipeline.
    /// </summary>
    /// <param name="configure">Action to configure the application builder.</param>
    public void UseMiddleware(Action<IApplicationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(Application);
    }

    /// <summary>
    /// Checks if the current environment is Development.
    /// </summary>
    /// <returns>True if the environment is Development; otherwise, false.</returns>
    public bool IsDevelopment() => Application.Environment.IsDevelopment();

    /// <summary>
    /// Checks if the current environment is Production.
    /// </summary>
    /// <returns>True if the environment is Production; otherwise, false.</returns>
    public bool IsProduction() => Application.Environment.IsProduction();
}