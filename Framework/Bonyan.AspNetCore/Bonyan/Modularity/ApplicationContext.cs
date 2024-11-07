namespace Bonyan.Modularity
{
    /// <summary>
    /// Context for managing application services and environment settings.
    /// </summary>
    public class BonContext : BonContextBase
    {
        public WebApplication Application { get; }

        public BonContext(WebApplication application)
            : base(application.Services, application.Configuration)
        {
            Application = application;
        }

        /// <summary>
        /// Registers middleware in the application pipeline.
        /// </summary>
        public void UseMiddleware(Action<IApplicationBuilder> configure)
        {
            configure(Application);
        }

        /// <summary>
        /// Checks if the current environment is Development.
        /// </summary>
        public bool IsDevelopment() => Application.Environment.IsDevelopment();

        /// <summary>
        /// Checks if the current environment is Production.
        /// </summary>
        public bool IsProduction() => Application.Environment.IsProduction();
    }
}