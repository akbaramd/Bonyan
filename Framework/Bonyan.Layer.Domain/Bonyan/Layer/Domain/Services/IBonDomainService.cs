using Bonyan.UnitOfWork;
using Bonyan.Validation;

namespace Bonyan.Layer.Domain.Services;

public interface IBonDomainService :IBonValidationEnabled,IBonUnitOfWorkEnabled
{

}