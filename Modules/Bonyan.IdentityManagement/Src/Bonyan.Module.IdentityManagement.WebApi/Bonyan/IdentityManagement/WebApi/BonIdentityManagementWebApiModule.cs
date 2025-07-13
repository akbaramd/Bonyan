using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.WebApi;

public class BonIdentityManagementWebApiModule : BonWebModule
{
    public BonIdentityManagementWebApiModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonIdentityManagementApplicationModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
  
        return base.OnPostApplicationAsync(context);
    }



}
