using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Validation;

public class BonyanValidationModule : Module
{
  public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
  {
    
    context.Services.AddTransient<IAttributeValidationResultProvider,DefaultAttributeValidationResultProvider>();
    context.Services.AddTransient<IObjectValidationContributor,DataAnnotationObjectValidationContributor>();
    context.Services.AddTransient<IMethodInvocationValidator,MethodInvocationValidator>();
    context.Services.AddTransient<IObjectValidator,ObjectValidator>();
    context.Services.AddTransient<ValidationInterceptor>();
    context.Services.OnRegistered(ValidationInterceptorRegistrar.RegisterIfNeeded);
    
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {

    return base.OnConfigureAsync(context);
  }

}