using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bonyan.IdentityManagement.Options;

public class BonIdentityManagementOptions
{
        private readonly BonConfigurationContext _context;
    
            public BonIdentityManagementOptions(BonConfigurationContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
      
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
    
         
}