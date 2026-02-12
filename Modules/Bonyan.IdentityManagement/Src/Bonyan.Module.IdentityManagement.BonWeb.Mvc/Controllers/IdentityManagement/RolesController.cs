using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.Layer.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Controllers.IdentityManagement;

[Area("IdentityManagement")]
[Authorize]
[Route("IdentityManagement/[controller]")]
public class RolesController : Controller
{
    private readonly IRoleAppService _roleAppService;

    public RolesController(IRoleAppService roleAppService)
    {
        _roleAppService = roleAppService;
    }

    [HttpGet]
    [Route("")]
    [Route("[action]")]
    public async Task<IActionResult> Index([FromQuery] RoleFilterDto? filter, CancellationToken cancellationToken = default)
    {
        var result = await _roleAppService.PaginatedAsync(filter ?? new RoleFilterDto(), cancellationToken);
        if (!result.Success)
            return View("Error", result.Message);
        var items = result.Data?.Results?.ToList() ?? new List<RoleDto>();
        return View(items);
    }
}
