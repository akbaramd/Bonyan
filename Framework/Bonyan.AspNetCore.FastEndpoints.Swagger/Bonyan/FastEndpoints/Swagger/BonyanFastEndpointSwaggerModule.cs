using Bonyan.Extensions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Bonyan.FastEndpoints.Swagger
{
  [DependOn(typeof(BonyanFastEndpointModule))]
  public class BonyanFastEndpointSwaggerModule : Module
  {
    private const int MinimumSigningKeyLength = 16; // Minimum 16 characters (128 bits) for HS256

    public override Task OnConfigureAsync(ModularityContext context)
    {
      
      DocumentOptions documentOptions;

      var options = context.RequireService<IOptions<DocumentOptions>>();
      documentOptions = options.Value;

      if (documentOptions == null)
      {
        throw new InvalidOperationException(
          $"SigningKey in JwtSigningOptions is not configured. Please configure it using context.Services.Configure<DocumentOptions>() in your main module. {nameof(IModule.OnPreConfigureAsync)}");
      }
      
      
      context.Services.SwaggerDocument(c =>
      {
        OptionsHelper.CopyProperties(c,documentOptions);
      });
      
      context.Services.Configure<FastEndpointConfiguration>(c =>
      {
        c.AddAfterInitializer(app =>
        {
          app.Application.UseSwaggerGen();
        });
      });
      return base.OnConfigureAsync(context);
    }
  }
}
