using Bonyan.DependencyInjection;

namespace Bonyan.Validation;

public class ValidationInterceptor : BonInterceptor
{
    private readonly IMethodInvocationValidator _methodInvocationValidator;

    public ValidationInterceptor(IMethodInvocationValidator methodInvocationValidator)
    {
        _methodInvocationValidator = methodInvocationValidator;
    }

    public override async Task InterceptAsync(IBonMethodInvocation invocation)
    {
        await ValidateAsync(invocation);
        await invocation.ProceedAsync();
    }

    protected virtual async Task ValidateAsync(IBonMethodInvocation invocation)
    {
        await _methodInvocationValidator.ValidateAsync(
            new MethodInvocationValidationContext(
                invocation.TargetObject,
                invocation.Method,
                invocation.Arguments
            )
        );
    }
}