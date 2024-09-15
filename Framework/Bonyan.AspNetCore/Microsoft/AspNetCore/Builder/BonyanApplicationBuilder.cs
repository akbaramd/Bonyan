using Bonyan.AspNetCore.Extensions;
using Bonyan.AspNetCore.Jobs;
using Hangfire;
using System.Collections.Generic;
using System;
using Bonyan.AspNetCore.Application;
using Bonyan.AspNetCore.Domain;
using Bonyan.AspNetCore.Infrastructure;

namespace Microsoft.AspNetCore.Builder
{
    public class BonyanApplicationBuilder : IBonyanApplicationBuilder
    {
        private readonly List<ConsoleMessage> _consoleMessages = new();
        private readonly List<(Type, string)> _cronJobTypes = new(); // Store cron jobs with their expressions
        private readonly List<Type> _backgroundJobTypes = new(); // Store background jobs
        private readonly List<Action<BonyanApplication>> _syncActions = new();
        private readonly List<Action<BonyanApplication>> _syncBeforeActions = new();
        private readonly WebApplicationBuilder _builder;

        public BonyanServiceInfo ServiceInfo { get; }
        public IServiceCollection Services { get; }
        public ConfigureHostBuilder Host { get; }
        public IConfiguration Configuration { get; }
        
        
        public IBonyanApplicationBuilder ConfigureDomain(Action<IDomainConfiguration> configure)
        {
          var  configuration = new DomainConfiguration(Configuration);
          configure.Invoke(configuration);
          return this;
        }

        public IBonyanApplicationBuilder ConfigureApplication(Action<IApplicationConfiguration> configure)
        {
          var  configuration = new ApplicationConfiguration();
          configure.Invoke(configuration);
          return this;
        }

        public IBonyanApplicationBuilder ConfigureInfrastructure(Action<IInfrastructureConfiguration> configure)
        {
          var  configuration = new InfrastructureConfiguration();
          configure.Invoke(configuration);
          return this;
        }

        public BonyanApplicationBuilder(BonyanServiceInfo serviceInfo, WebApplicationBuilder builder)
        {
            ServiceInfo = serviceInfo;
            Services = builder.Services;
            Host = builder.Host;
            Configuration = builder.Configuration;
            _builder = builder;
        }

        public IBonyanApplicationBuilder AddBeforeInitializer(Action<BonyanApplication> action)
        {
          _syncBeforeActions.Add(action);
          return this;
        }

        public IBonyanApplicationBuilder AddInitializer(Action<BonyanApplication> action)
        {
            _syncActions.Add(action);
            return this;
        }

        public IBonyanApplicationBuilder AddBeforeInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer
        {
          Services.AddSingleton<TInitializer>();
          _syncBeforeActions.Add(app =>
          {
            try
            {
              var initializer = app.Application.Services.GetRequiredService<TInitializer>();
              initializer.InitializeAsync(app).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
              Console.WriteLine($"Failed to initialize {typeof(TInitializer).Name}: {ex.Message}");
            }
          });
          return this;
        }

        public IBonyanApplicationBuilder AddInitializer<TInitializer>() where TInitializer : class, IBonyanApplicationInitializer
        {
            Services.AddSingleton<TInitializer>();
            _syncActions.Add(app =>
            {
                try
                {
                    var initializer = app.Application.Services.GetRequiredService<TInitializer>();
                    initializer.InitializeAsync(app).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize {typeof(TInitializer).Name}: {ex.Message}");
                }
            });
            return this;
        }

        // Method for adding cron jobs
        public IBonyanApplicationBuilder AddCronJob<TJob>(string cronExpression) where TJob : class, IJob
        {
            _cronJobTypes.Add((typeof(TJob), cronExpression));
            Services.AddTransient<TJob>(); // Register the job type
            return this;
        }

        // Method for adding background jobs
        public IBonyanApplicationBuilder AddBackgroundJob<TJob>() where TJob : class, IJob
        {
            _backgroundJobTypes.Add(typeof(TJob));
            Services.AddTransient<TJob>(); // Register the job type
            return this;
        }

        public IBonyanApplicationBuilder AddConsoleMessage(string message, string category = "Info")
        {
            _consoleMessages.Add(new ConsoleMessage(message, category));
            return this;
        }

        public BonyanApplication Build(Action<BonyanApplication>? builder = null)
        {
            Services.AddHangfire(config => config.UseInMemoryStorage());
            Services.AddHangfireServer();

            var application = _builder.Build();
            var bonyanApplication = new BonyanApplication(application, ServiceInfo) { ConsoleMessages = _consoleMessages };
            foreach (var action in _syncBeforeActions)
            {
              action.Invoke(bonyanApplication);
            }
            foreach (var action in _syncActions)
            {
                action.Invoke(bonyanApplication);
            }

            application.UseHangfireDashboard();

            // Register all jobs after adding Hangfire services
            using (var scope = application.Services.CreateScope())
            {
                var jobManager = scope.ServiceProvider.GetRequiredService<IOptimumJobsManager>();
              
                // Register cron jobs
                foreach (var (jobType, cronExpression) in _cronJobTypes)
                {
                    var addCronJobMethod = typeof(IOptimumJobsManager).GetMethod("AddCronJob")?.MakeGenericMethod(jobType);
                    addCronJobMethod?.Invoke(jobManager, new object[] { cronExpression, scope });
                    
                    
                    // Add a message for each cron job
                    _consoleMessages.Add(new ConsoleMessage(
                    
                       $"Registered cron job: {jobType.Name} with cron expression: {cronExpression}",
                       "CronJob"
                    ));
                }

                // Register background jobs
                foreach (var jobType in _backgroundJobTypes)
                {
                    var addBackgroundJobMethod = typeof(IOptimumJobsManager).GetMethod("AddBackgroundJob")?.MakeGenericMethod(jobType);
                    addBackgroundJobMethod?.Invoke(jobManager, new object[] { scope });
                    
                    // Add a message for each background job
                    _consoleMessages.Add( new ConsoleMessage($"Registered background job: {jobType.Name}",
                      "BackgroundJob"));
                }
            }
            builder?.Invoke(bonyanApplication);
            return bonyanApplication;
        }
    }
}
