namespace Bonyan.Modularity
{
    /// <summary>
    /// Context for managing application services and environment settings.
    /// </summary>
    public class BonWebApplicationContext : BonContextBase
    {
        public WebApplication Application { get; }

        public BonWebApplicationContext(WebApplication application)
            : base(application.Services)
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