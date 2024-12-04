using Bonyan.AspNetCore.Authorization;
using Bonyan.AspNetCore.Mvc;
using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Application.Permissions;
using Bonyan.IdentityManagement.Application.Auth;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
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

namespace Bonyan.IdentityManagement.WebApi;

public class BonIdentityManagementWebApiModule<TUser> : BonWebModule
    where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementWebApiModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
        DependOn<BonIdentityManagementApplicationModule<TUser>>();
    }

    private void RegisterPermissions()
    {
        PreConfigure<BonAuthorizationConfiguration>(config =>
        {
            config.RegisterPermissions(new[]
            {
                BonIdentityPermissionConstants.IdentityPermissionRead,
                BonIdentityPermissionConstants.IdentityRoleRead,
                BonIdentityPermissionConstants.IdentityRoleDelete,
                BonIdentityPermissionConstants.IdentityRoleEdit,
                BonIdentityPermissionConstants.IdentityRoleCreate,
                BonIdentityPermissionConstants.IdentityUserRead,
                BonIdentityPermissionConstants.IdentityUserDelete,
                BonIdentityPermissionConstants.IdentityUserEdit,
                BonIdentityPermissionConstants.IdentityUserCreate
            });
        });
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        RegisterPermissions();
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        ConfigurePermissionEndpoints(context);
        ConfigureAuthEndpoints(context);
        ConfigureRoleEndpoints(context);
        return base.OnPostApplicationAsync(context);
    }

    private void ConfigurePermissionEndpoints(BonWebApplicationContext context)
    {
        var group = context.Application.MapGroup("/api/identity/permissions").WithTags("Permission");

        group.MapGet("/paginate", async ([FromServices] IBonIdentityPermissionAppService service,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10,
            [FromQuery] string? search = null) =>
            await service.PaginatedAsync(new BonFilterAndPaginateDto { Skip = skip, Take = take, Search = search }))
            .Produces<ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityPermissionRead);

        group.MapGet("/detail/{id}", async ([FromServices] IBonIdentityPermissionAppService service,
            [FromRoute] string id) =>
            await service.DetailAsync(BonPermissionId.NewId(id)))
            .Produces<ServiceResult<BonIdentityPermissionDto>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityPermissionRead);
    }

    private void ConfigureAuthEndpoints(BonWebApplicationContext context)
    {
        var group = context.Application.MapGroup("/api/identity/auth").WithTags("Authentication");

        group.MapPost("/register", async ([FromServices] IBonIdentityAuthAppService service,
            [FromBody] BonIdentityUserRegistererDto registerDto) =>
            await service.RegisterAsync(registerDto))
            .Produces<ServiceResult<bool>>()
            .AllowAnonymous();

        group.MapGet("/profile", async ([FromServices] IBonIdentityAuthAppService service) =>
            await service.ProfileAsync())
            .Produces<ServiceResult<BonIdentityUserDto>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityUserRead);

        group.MapPost("/signin/cookie", async ([FromServices] IBonIdentityAuthAppService service,
            [FromBody] BonIdentityUserCookieSignInDto signInDto) =>
            await service.CookieSignInAsync(signInDto.Username, signInDto.Password, signInDto.IsPersistent))
            .Produces<ServiceResult<bool>>()
            .AllowAnonymous();

        group.MapPost("/signin/jwt", async ([FromServices] IBonIdentityAuthAppService service,
            [FromBody] BonIdentityUserJwtSignInDto signInDto) =>
            await service.JwtBearerSignInAsync(signInDto.Username, signInDto.Password))
            .Produces<ServiceResult<BonIdentityJwtResultDto>>()
            .AllowAnonymous();

        group.MapPost("/refresh-token", async ([FromServices] IBonIdentityAuthAppService service,
            [FromBody] BonIdentityUserRefreshTokenDto refreshToken) =>
            await service.RefreshTokenAsync(refreshToken.RefreshToken))
            .Produces<ServiceResult<BonIdentityJwtResultDto>>()
            .AllowAnonymous();
    }

    private void ConfigureRoleEndpoints(BonWebApplicationContext context)
    {
        var group = context.Application.MapGroup("/api/identity/roles").WithTags("Roles");

        group.MapGet("/paginate", async ([FromServices] IBonIdentityRoleAppService service,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10,
            [FromQuery] string? search = null) =>
            await service.PaginatedAsync(new BonFilterAndPaginateDto { Skip = skip, Take = take, Search = search }))
            .Produces<ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityRoleRead);

        group.MapGet("/detail/{id}", async ([FromServices] IBonIdentityRoleAppService service,
            [FromRoute] string id) =>
            await service.DetailAsync(BonRoleId.NewId(id)))
            .Produces<ServiceResult<BonIdentityRoleDto>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityRoleRead);

        group.MapPost("/create", async ([FromServices] IBonIdentityRoleAppService service,
            [FromBody] BonIdentityRoleCreateDto createDto) =>
            await service.CreateAsync(createDto))
            .Produces<ServiceResult<BonIdentityRoleDto>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityRoleCreate);

        group.MapPut("/update/{id}", async ([FromServices] IBonIdentityRoleAppService service,
            [FromRoute] string id,
            [FromBody] BonIdentityRoleUpdateDto updateDto) =>
            await service.UpdateAsync(BonRoleId.NewId(id), updateDto))
            .Produces<ServiceResult<BonIdentityRoleDto>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityRoleEdit);

        group.MapDelete("/delete/{id}", async ([FromServices] IBonIdentityRoleAppService service,
            [FromRoute] string id) =>
            await service.DeleteAsync(BonRoleId.NewId(id)))
            .Produces<ServiceResult<bool>>()
            .RequireAuthorization(BonIdentityPermissionConstants.IdentityRoleDelete);
    }
}
