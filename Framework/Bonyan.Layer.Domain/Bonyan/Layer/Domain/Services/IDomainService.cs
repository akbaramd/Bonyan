using Bonyan.DependencyInjection;
using Bonyan.UnitOfWork;
using Bonyan.Validation;

namespace Bonyan.Layer.Domain.Services;

public interface IDomainService :IValidationEnabled,IUnitOfWorkEnabled
{

}