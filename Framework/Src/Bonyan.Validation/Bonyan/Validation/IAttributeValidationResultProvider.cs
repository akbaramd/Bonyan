using System.ComponentModel.DataAnnotations;

namespace Bonyan.Validation;

public interface IAttributeValidationResultProvider
{
    ValidationResult? GetOrDefault(ValidationAttribute validationAttribute, object? validatingObject, ValidationContext validationContext);
}
