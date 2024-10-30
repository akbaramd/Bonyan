using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.Validation;

namespace Bonyan.Layer.Application.Services;

public interface IApplicationService : IValidationEnabled,IUnitOfWorkEnabled
{
    public ICurrentUser CurrentUser { get;  }
    public ICurrentTenant CurrentTenant { get;  }
}