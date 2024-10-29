using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.User;

namespace Bonyan.Layer.Application.Services;

public interface IApplicationService
{
    public ICurrentUser CurrentUser { get;  }
    public ICurrentTenant CurrentTenant { get;  }
}