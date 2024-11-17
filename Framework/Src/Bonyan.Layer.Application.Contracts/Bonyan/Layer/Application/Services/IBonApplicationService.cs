using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.Validation;

namespace Bonyan.Layer.Application.Abstractions;

public interface IBonApplicationService : IBonValidationEnabled,IBonUnitOfWorkEnabled,IBonLayServiceProviderConfigurator
{
    public IBonCurrentUser BonCurrentUser { get;  }
    public IBonCurrentTenant BonCurrentTenant { get;  }
}