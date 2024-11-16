using System.ComponentModel.DataAnnotations;

namespace Bonyan.Validation;

public class DefaultAttributeValidationResultProvider : IAttributeValidationResultProvider
{
    public virtual ValidationResult? GetOrDefault(ValidationAttribute validationAttribute, object? validatingObject, ValidationContext validationContext)
    {
        return validationAttribute.GetValidationResult(validatingObject, validationContext);
    }
}
