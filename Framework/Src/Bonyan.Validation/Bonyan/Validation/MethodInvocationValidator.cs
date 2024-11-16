using System.Reflection;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Helpers;

namespace Bonyan.Validation;

public class MethodInvocationValidator : IMethodInvocationValidator
{
    private readonly IObjectValidator _objectValidator;

    public MethodInvocationValidator(IObjectValidator objectValidator)
    {
        _objectValidator = objectValidator;
    }

    public virtual async Task ValidateAsync(MethodInvocationValidationContext context)
    {
        Check.NotNull(context, nameof(context));

        if (context.Parameters.IsNullOrEmpty())
        {
            return;
        }

        if (!context.Method.IsPublic)
        {
            return;
        }

        if (IsValidationDisabled(context))
        {
            return;
        }

        if (context.Parameters.Length != context.ParameterValues.Length)
        {
            throw new BonException("Method parameter count does not match with argument count!");
        }

        //todo: consider to remove this condition
        if (context.Errors.Any() && HasSingleNullArgument(context))
        {
            ThrowValidationError(context);
        }

        await AddMethodParameterValidationErrorsAsync(context);

        if (context.Errors.Any())
        {
            ThrowValidationError(context);
        }
    }

    protected virtual bool IsValidationDisabled(MethodInvocationValidationContext context)
    {
        if (context.Method.IsDefined(typeof(EnableValidationAttribute), true))
        {
            return false;
        }

        if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableValidationAttribute>(context.Method) != null)
        {
            return true;
        }

        return false;
    }

    protected virtual bool HasSingleNullArgument(MethodInvocationValidationContext context)
    {
        return context.Parameters.Length == 1 && context.ParameterValues[0] == null;
    }

    protected virtual void ThrowValidationError(MethodInvocationValidationContext context)
    {
        throw new BonException(
            "Method arguments are not valid! See ValidationErrors for details."
         
        );
    }

    protected virtual async Task AddMethodParameterValidationErrorsAsync(MethodInvocationValidationContext context)
    {
        for (var i = 0; i < context.Parameters.Length; i++)
        {
            await AddMethodParameterValidationErrorsAsync(context, context.Parameters[i], context.ParameterValues[i]);
        }
    }

    protected virtual async Task AddMethodParameterValidationErrorsAsync(IBonyanValidationResult context, ParameterInfo parameterInfo, object parameterValue)
    {
        var allowNulls = parameterInfo.IsOptional ||
                         parameterInfo.IsOut ||
                         TypeHelper.IsPrimitiveExtended(parameterInfo.ParameterType, includeEnums: true);

        context.Errors.AddRange(
             await _objectValidator.GetErrorsAsync(
                parameterValue,
                parameterInfo.Name,
                allowNulls
            )
        );
    }
}
