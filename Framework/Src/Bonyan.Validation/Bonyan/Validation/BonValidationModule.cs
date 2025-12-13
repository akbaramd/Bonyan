using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Validation;

public class BonValidationModule : BonModule
{
  public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
  {
    
    context.Services.AddTransient<IAttributeValidationResultProvider,DefaultAttributeValidationResultProvider>();
    context.Services.AddTransient<IObjectValidationContributor,DataAnnotationObjectValidationContributor>();
    context.Services.AddTransient<IMethodInvocationValidator,MethodInvocationValidator>();
    context.Services.AddTransient<IObjectValidator,ObjectValidator>();
    context.Services.AddTransient<ValidationInterceptor>();
    context.Services.OnRegistered(ValidationInterceptorRegistrar.RegisterIfNeeded);
    
    return base.OnPreConfigureAsync(context, cancellationToken);
  }

  public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
  {

    return base.OnConfigureAsync(context, cancellationToken);
  }

}