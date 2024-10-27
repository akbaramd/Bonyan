

namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationBuilder 
{

 IConfigurationManager Configuration { get; }

 IHostEnvironment Environment { get; }

 ILoggingBuilder Logging { get; }

 IServiceCollection Services { get; }

 ConfigureHostBuilder Host { get; }

 WebApplication Build();
}
