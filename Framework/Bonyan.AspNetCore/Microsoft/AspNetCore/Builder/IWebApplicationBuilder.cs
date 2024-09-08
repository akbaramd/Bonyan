using Bonyan.AspNetCore.Jobs;

namespace Microsoft.AspNetCore.Builder
{
  public interface IBonyanApplicationBuilder
  {
    IServiceCollection Services { get; }
    ConfigureHostBuilder Host { get; }
    IConfiguration Configuration { get; }

    // Synchronous methods
    IBonyanApplicationBuilder AddInitializer(Action<BonyanApplication> action);
    IBonyanApplicationBuilder AddInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer;
    IBonyanApplicationBuilder AddConsoleMessage(string message, string category = "Info");
    IBonyanApplicationBuilder AddCronJob<TJob>(string cronExpression) where TJob : class, IJob;
    IBonyanApplicationBuilder AddBackgroundJob<TJob>() where TJob : class, IJob;
    BonyanApplication Build();

  }
}
