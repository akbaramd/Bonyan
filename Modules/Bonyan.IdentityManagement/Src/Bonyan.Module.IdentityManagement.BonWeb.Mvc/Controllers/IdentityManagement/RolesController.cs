using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Controllers.IdentityManagement;

[Area("IdentityManagement")]
[Authorize]
[Route("IdentityManagement/[controller]")]
public class RolesController : Controller
{
    private readonly IRoleAppService _roleAppService;
    private readonly IStringLocalizer<IdentityManagementResource> _localizer;

    public RolesController(IRoleAppService roleAppService, IStringLocalizer<IdentityManagementResource> localizer)
    {
        _roleAppService = roleAppService;
        _localizer = localizer;
    }

    [HttpGet]
    [Route("")]
    [Route("[action]")]
    public async Task<IActionResult> Index([FromQuery] RoleFilterDto? filter, CancellationToken cancellationToken = default)
    {
        var filterDto = filter ?? new RoleFilterDto();
        var result = await _roleAppService.PaginatedAsync(filterDto);
        if (!result.IsSuccess)
            return View("Error", _localizer["Error:Message", result.ErrorMessage ?? ""].Value);
        ViewBag.Filter = filterDto;
        var model = result.Result ?? new BonPaginatedResult<RoleDto>(Array.Empty<RoleDto>(), 0, filterDto.Take, 0);
        return View(model);
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] RoleCreateDto input, CancellationToken cancellationToken = default)
    {
        var result = await _roleAppService.CreateAsync(input);
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Roles:CreateFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Roles:CreateSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] string id, [FromForm] RoleUpdateDto input, CancellationToken cancellationToken = default)
    {
        var roleId = BonRoleId.NewId(id);
        var result = await _roleAppService.UpdateAsync(roleId, input);
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Roles:UpdateFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Roles:UpdateSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([FromForm] string id, CancellationToken cancellationToken = default)
    {
        var roleId = BonRoleId.NewId(id);
        var result = await _roleAppService.DeleteAsync(roleId);
        if (!result.IsSuccess)
            TempData["Error"] = result.ErrorCode == "CannotDelete"
                ? _localizer["Roles:DeleteForbidden"].Value
                : (result.ErrorMessage ?? _localizer["Roles:DeleteFailed"].Value);
        else
            TempData["Success"] = _localizer["Roles:DeleteSuccess"].Value;
        return RedirectToAction(nameof(Index));
    }
}
