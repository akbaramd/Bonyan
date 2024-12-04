using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Bonyan.Layer.Application;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapReadonlyEndpoints<TKey, TFilterDto, TDto, TDetailDto, TService>(
        this RouteGroupBuilder group, Func<string,TKey> convert , string routePrefix = "" )
        where TService : IBonReadonlyAppService<TKey, TFilterDto, TDto, TDetailDto>
        where TDto : class
        where TDetailDto : class
        where TFilterDto : BonFilterAndPaginateDto, new()
    {
        // تنظیم مسیرها با در نظر گرفتن پیشوند (اگر وجود داشته باشد)
        var paginateRoute = string.IsNullOrEmpty(routePrefix) ? "/paginate" : $"{routePrefix}/paginate";
        var detailRoute = string.IsNullOrEmpty(routePrefix) ? "/detail/{id}" : $"{routePrefix}/detail/{{id}}";

        // MapGet برای paginate
        group.MapGet(paginateRoute, async ([FromServices] TService service,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10,
            [FromQuery] string? search = null) =>
        {
            var filterDto = new TFilterDto
            {
                Skip = skip,
                Take = take,
                Search = search
            };

            return await service.PaginatedAsync(filterDto);
        }).Produces<ServiceResult<BonPaginatedResult<TDto>>>();

        // MapGet برای detail
        group.MapGet(detailRoute, async ([FromServices] TService service,
                [FromRoute] string id) => await service.DetailAsync(convert.Invoke(id)))
            .Produces<ServiceResult<TDetailDto>>()
            ;

        return group;
    }
}