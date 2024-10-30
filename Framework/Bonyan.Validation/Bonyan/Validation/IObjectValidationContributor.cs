namespace Bonyan.Validation;

public interface IObjectValidationContributor
{
    Task AddErrorsAsync(ObjectValidationContext context);
}
