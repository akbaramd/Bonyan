using Bonyan.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Novin.AspNetCore.Novin.AspNetCore.Extensions;

namespace Novin.AspNetCore.Novin.AspNetCore
{
    public class NovinApplication
    {
        internal WebApplication App { get; set; }

        public NovinApplication(WebApplication app)
        {
            App = app;
        }

        public static NovinApplicationBuilder CreateBuilder(string serviceName, Action<NovinConfigurationContext> configure)
        {
          

            var applicationBuilder = BonyanApplication.CreateModularBuilder<NovinModule>(serviceName,c =>
            {
                var context = new NovinConfigurationContext(c);
                
                configure.Invoke(context);
            });
            
            // Add Swagger to the services collection
            applicationBuilder.Services.AddEndpointsApiExplorer();
            applicationBuilder.Services.AddSwaggerGen(options =>
            {
                // Basic Swagger setup - customize as needed
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{serviceName} API",
                    Version = "v1",
                    Description = "API documentation for the service"
                });
            });

            return new NovinApplicationBuilder(applicationBuilder);
        }

        public Task RunAsync()
        {
           

            return App.RunAsync();
        }
    }
}