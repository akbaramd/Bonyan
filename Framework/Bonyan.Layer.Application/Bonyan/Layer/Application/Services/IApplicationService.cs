using Bonyan.MultiTenant;
using Bonyan.User;

namespace Bonyan.Layer.Application.Services;

public interface IApplicationService
{
    public IServiceProvider ServiceProvider { get;  }
    public ICurrentUser CurrentUser { get;  }
    public ICurrentTenant CurrentTenant { get;  }
}