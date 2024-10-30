using System.ComponentModel.DataAnnotations;

namespace Bonyan.Validation;

public class BonyanValidationResult : IBonyanValidationResult
{
    public List<ValidationResult> Errors { get; }

    public BonyanValidationResult()
    {
        Errors = new List<ValidationResult>();
    }
}
