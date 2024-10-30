namespace Bonyan.Validation;

public interface IMethodInvocationValidator
{
    Task ValidateAsync(MethodInvocationValidationContext context);
}
