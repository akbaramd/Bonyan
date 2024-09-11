using FastEndpoints;
using FastEndpoints.Swagger;

namespace Microsoft.AspNetCore.Builder;

public  static class FastEndpointsBonyanApplicationExtensions
{
  public static BonyanApplication UseFastEndpoints(this BonyanApplication application)
  {
    application.Application.UseFastEndpoints();
    application.Application.UseSwaggerGen();
  
    
    
    return application;
  }
}
