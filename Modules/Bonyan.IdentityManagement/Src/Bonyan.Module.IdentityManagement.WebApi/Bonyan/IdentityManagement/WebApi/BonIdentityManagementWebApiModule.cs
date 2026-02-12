using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Domain.Roles;
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

public class BonIdentityManagementWebApiModule: Modularity.Abstractions.BonModule 
{
    public BonIdentityManagementWebApiModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonIdentityManagementApplicationModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        return base.OnConfigureAsync(context);
    }





}
