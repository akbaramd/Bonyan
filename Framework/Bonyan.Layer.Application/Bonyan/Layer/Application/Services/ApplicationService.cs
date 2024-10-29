using AutoMapper;
using Bonyan.DependencyInjection;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Application.Services;

public class ApplicationService : IApplicationService
{
  public IBonyanLazyServiceProvider ServiceProvider { get; set; } = default!;
  public ICurrentUser CurrentUser => ServiceProvider.LazyGetRequiredService<ICurrentUser>();
  public ICurrentTenant CurrentTenant => ServiceProvider.LazyGetRequiredService<ICurrentTenant>();
  public IMapper Mapper => ServiceProvider.LazyGetRequiredService<IMapper>();
  public IUnitOfWork UnitOfWork => ServiceProvider.LazyGetRequiredService<IUnitOfWork>();
}
