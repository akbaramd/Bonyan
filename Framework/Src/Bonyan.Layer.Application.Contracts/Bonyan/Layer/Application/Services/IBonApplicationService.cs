using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.Validation;

namespace Bonyan.Layer.Application.Services;

public interface IBonApplicationService : IBonValidationEnabled,IBonLazyServiceProviderConfigurator
{
    public IBonCurrentUser BonCurrentUser { get;  }
    public IBonCurrentTenant BonCurrentTenant { get;  }
}