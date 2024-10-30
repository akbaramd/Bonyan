namespace Bonyan.UnitOfWork;

public class BonyanAspNetCoreUnitOfWorkOptions
{
    /// <summary>
    /// This is used to disable the <see cref="BonyanUnitOfWorkMiddleware"/>,
    /// app.UseUnitOfWork(), for the specified URLs.
    /// <see cref="BonyanUnitOfWorkMiddleware"/> will be disabled for URLs
    /// starting with an ignored URL.  
    /// </summary>
    public List<string> IgnoredUrls { get; } = new List<string>();
}
