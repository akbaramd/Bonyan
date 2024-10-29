using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Application.Services;

public class ApplicationService : IApplicationService
{
  public IBonyanLazyServiceProvider ServiceProvider { get; } = default!;
  public ICurrentUser CurrentUser => ServiceProvider.LazyGetRequiredService<ICurrentUser>();
  public ICurrentTenant CurrentTenant => ServiceProvider.LazyGetRequiredService<ICurrentTenant>();
}
