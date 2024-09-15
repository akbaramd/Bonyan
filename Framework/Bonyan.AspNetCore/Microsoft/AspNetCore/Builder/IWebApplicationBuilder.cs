using Bonyan.AspNetCore.Application;
using Bonyan.AspNetCore.Domain;
using Bonyan.AspNetCore.Infrastructure;
using Bonyan.AspNetCore.Jobs;

namespace Microsoft.AspNetCore.Builder;

public interface IBonyanApplicationBuilder
{
 protected internal IServiceCollection Services { get; }
 protected internal ConfigureHostBuilder Host { get; }
 protected internal IConfiguration Configuration { get; }


 IBonyanApplicationBuilder ConfigureDomain(Action<IDomainConfiguration> configure);
 IBonyanApplicationBuilder ConfigureApplication(Action<IApplicationConfiguration> configure);
 IBonyanApplicationBuilder ConfigureInfrastructure(Action<IInfrastructureConfiguration> configure);
 
 
 public IServiceCollection GetServicesCollection() => Services;
 public IConfiguration GetConfiguration() => Configuration;
 
  // Synchronous methods
  IBonyanApplicationBuilder AddBeforeInitializer(Action<BonyanApplication> action);
  IBonyanApplicationBuilder AddInitializer(Action<BonyanApplication> action);
  IBonyanApplicationBuilder AddBeforeInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer;
  IBonyanApplicationBuilder AddInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer;
  IBonyanApplicationBuilder AddConsoleMessage(string message, string category = "Info");
  IBonyanApplicationBuilder AddCronJob<TJob>(string cronExpression) where TJob : class, IJob;
  IBonyanApplicationBuilder AddBackgroundJob<TJob>() where TJob : class, IJob;
  BonyanApplication Build(Action<BonyanApplication>? builder = null);
}
