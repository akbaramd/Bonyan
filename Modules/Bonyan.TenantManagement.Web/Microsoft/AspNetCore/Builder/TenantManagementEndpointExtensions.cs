using Bonyan.Layer.Domain.Abstractions.Results;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Application.Services;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

public static class TenantManagementEndpointExtensions
{
    public static IEndpointRouteBuilder MapTenantManagementEndpoints(this IEndpointRouteBuilder builder)
    {
        var options = builder.ServiceProvider.GetRequiredService<IOptions<TenantManagementEndpointOptions>>().Value;
        builder.MapGet(options.BaseEndpoint + "/paginate", async (
                [FromQuery] string? search,
                [FromQuery] int take,
                [FromQuery] int skip,
                [FromServices] IBonTenantBonApplicationService tenantApplicationService
            ) => await tenantApplicationService.PaginateAsync(new BonTenantFilterDto()
            {
                Search = search,
                Skip = skip,
                Take = take
            }))
            .Produces<BonPaginatedResult<BonTenantDto>>();

        builder.MapGet(options.BaseEndpoint + "/detail/{id:guid}", async (
                Guid id,
                [FromServices] IBonTenantBonApplicationService tenantApplicationService
            ) => await tenantApplicationService.DetailAsync(BonTenantId.FromGuid(id)))
            .Produces<BonTenantDto>();

        builder.MapPut(options.BaseEndpoint, async (
                [FromBody] BonTenantCreateDto dto,
                [FromServices] IBonTenantBonApplicationService tenantApplicationService
            ) => await tenantApplicationService.CreateAsync(dto))
            .Produces<BonTenantDto>();

        builder.MapPut(options.BaseEndpoint + "/{id:guid}", async (
                Guid id,
                [FromBody] BonTenantUpdateDto dto,
                [FromServices] IBonTenantBonApplicationService tenantApplicationService
            ) => await tenantApplicationService.UpdateAsync(BonTenantId.FromGuid(id), dto))
            .Produces<BonTenantDto>();

        builder.MapPut(options.BaseEndpoint + "/{id:guid}", async (
                Guid id,
                [FromServices] IBonTenantBonApplicationService tenantApplicationService
            ) => await tenantApplicationService.DeleteAsync(BonTenantId.FromGuid(id)))
            .Produces<BonTenantDto>();

        return builder;
    }
}