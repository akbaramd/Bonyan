using Bonyan.AspNetCore.Authorization.Permissions;
using Bonyan.Modularity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bonyan.AspNetCore.Authorization
{
    public class BonAuthorizationConfiguration
    {
        private readonly BonConfigurationContext _context;

        public BonAuthorizationConfiguration(BonConfigurationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.Services.AddObjectAccessor(new PermissionAccessor());
        }

        public void AddJwtAuthToSwagger()
        {
            _context.Services.PreConfigure<SwaggerGenOptions>(options =>
            {
                // Add the Bearer token definition
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and your token."
                });

                // Add a security requirement for all endpoints
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>() // No scopes required for this example
                    }
                });
            });
        }

        public void RegisterPermissions(string[] permissions)
        {
            // Register PermissionAccessor as a singleton
            _context.Services.GetObject<PermissionAccessor>().AddRange(permissions);
        }
    }
}
