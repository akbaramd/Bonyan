using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public class ModularityContext
{
  public IServiceCollection Services { get; set; }

  public IConfiguration Configuration { get; set; }

  public ModularityContext(IServiceCollection services, IConfiguration configuration)
  {
    Services = services;
    Configuration = configuration;
  }
  
  
    /// <summary>
        /// دریافت سرویس از طریق یک ServiceProvider موقت.
        /// </summary>
        public T? GetService<T>()
        {
          var serviceProvider = Services.BuildServiceProvider().CreateScope().ServiceProvider;
            return serviceProvider.GetService<T>();
        }

        /// <summary>
        /// دریافت سرویس اجباری از طریق یک ServiceProvider موقت.
        /// </summary>
        public T RequireService<T>() where T : notnull
        {
            var serviceProvider = Services.BuildServiceProvider().CreateScope().ServiceProvider;
            return serviceProvider.GetRequiredService<T>();
        }
}
