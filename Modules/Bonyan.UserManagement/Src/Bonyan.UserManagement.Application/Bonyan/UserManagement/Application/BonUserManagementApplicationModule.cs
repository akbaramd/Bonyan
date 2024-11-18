using Bonyan.AutoMapper;
using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application.Users;
using Bonyan.UserManagement.Application.Users.Services;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UserManagement.Application;


public class BonUserManagementApplicationModule<TUser> : BonModule where TUser : class, IBonUser
{
  public BonUserManagementApplicationModule()
  {
    DependOn<BonLayerApplicationModule>();
    DependOn<BonUserManagementDomainModule<TUser>>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {

    context.Services.Configure<BonAutoMapperOptions>(c =>
    {
      c.AddProfile<BonUserProfile>();
    });
    
    context.Services.AddTransient<BonUserCrudAppService<TUser>>();
    context.Services.AddTransient<BonUserReadOnlyAppService<TUser>>();
    context.Services.AddTransient<IBonUserCrudAppService>(sp=>sp.GetRequiredService<BonUserCrudAppService<TUser>>());
    context.Services.AddTransient<IBonUserReadOnlyAppService>(sp=>sp.GetRequiredService<BonUserReadOnlyAppService<TUser>>());
    
    
    return base.OnConfigureAsync(context);
  }
}
