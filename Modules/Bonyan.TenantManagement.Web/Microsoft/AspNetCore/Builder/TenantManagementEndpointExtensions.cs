using Bonyan.Layer.Domain.Model;
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
                [FromServices] ITenantApplicationService tenantApplicationService
            ) => await tenantApplicationService.PaginateAsync(new TenantFilterDto()
            {
                Search = search,
                Skip = skip,
                Take = take
            }))
            .Produces<PaginatedResult<TenantDto>>();

        builder.MapGet(options.BaseEndpoint + "/detail/{id:guid}", async (
                Guid id,
                [FromServices] ITenantApplicationService tenantApplicationService
            ) => await tenantApplicationService.DetailAsync(TenantId.FromGuid(id)))
            .Produces<TenantDto>();

        builder.MapPut(options.BaseEndpoint, async (
                [FromBody] TenantCreateDto dto,
                [FromServices] ITenantApplicationService tenantApplicationService
            ) => await tenantApplicationService.CreateAsync(dto))
            .Produces<TenantDto>();

        builder.MapPut(options.BaseEndpoint + "/{id:guid}", async (
                Guid id,
                [FromBody] TenantUpdateDto dto,
                [FromServices] ITenantApplicationService tenantApplicationService
            ) => await tenantApplicationService.UpdateAsync(TenantId.FromGuid(id), dto))
            .Produces<TenantDto>();

        builder.MapPut(options.BaseEndpoint + "/{id:guid}", async (
                Guid id,
                [FromServices] ITenantApplicationService tenantApplicationService
            ) => await tenantApplicationService.DeleteAsync(TenantId.FromGuid(id)))
            .Produces<TenantDto>();

        return builder;
    }
}