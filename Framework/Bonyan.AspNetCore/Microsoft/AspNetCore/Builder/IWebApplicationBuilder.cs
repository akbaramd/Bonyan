using Bonyan.AspNetCore.Jobs;

namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationBuilder
{
 protected internal IServiceCollection Services { get; }
 protected internal ConfigureHostBuilder Host { get; }
 protected internal IConfiguration Configuration { get; }


 public IServiceCollection GetServicesCollection() => Services;
 public IConfiguration GetConfiguration() => Configuration;
 
  // Synchronous methods
  IBonyanApplicationBuilder AddInitializer(Action<BonyanApplication> action);
  IBonyanApplicationBuilder AddInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer;
  IBonyanApplicationBuilder AddConsoleMessage(string message, string category = "Info");
  IBonyanApplicationBuilder AddCronJob<TJob>(string cronExpression) where TJob : class, IJob;
  IBonyanApplicationBuilder AddBackgroundJob<TJob>() where TJob : class, IJob;
  BonyanApplication Build(Action<BonyanApplication>? builder = null);
}
