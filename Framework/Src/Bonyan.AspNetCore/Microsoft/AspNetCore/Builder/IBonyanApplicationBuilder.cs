

using Bonyan.Modularity;

namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationBuilder 
{

 IConfigurationManager Configuration { get; }

 IHostEnvironment Environment { get; }

 ILoggingBuilder Logging { get; }

 IServiceCollection Services { get; }

 IHostBuilder Host { get; }

 Task<WebApplication> BuildAsync(Action<BonWebApplicationContext>? action = null);
}
