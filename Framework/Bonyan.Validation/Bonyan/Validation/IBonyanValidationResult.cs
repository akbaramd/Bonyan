using System.ComponentModel.DataAnnotations;

namespace Bonyan.Validation;

public interface IBonyanValidationResult
{
    List<ValidationResult> Errors { get; }
}
