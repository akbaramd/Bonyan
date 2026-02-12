using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Application.UserMeta;
using Bonyan.IdentityManagement.Application.Users;
using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.BonWeb.Mvc.Models.Manage;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Controllers.IdentityManagement;

[Area("IdentityManagement")]
[Authorize]
[Route("IdentityManagement/[controller]")]
public class UsersController : Controller
{
    private readonly IIdentityUserListAppService _userListAppService;
    private readonly IIdentityUserAppService _identityUserAppService;
    private readonly IRoleAppService _roleAppService;
    private readonly IUserMetaService _userMetaService;
    private readonly IStringLocalizer<IdentityManagementResource> _localizer;

    public UsersController(
        IIdentityUserListAppService userListAppService,
        IIdentityUserAppService identityUserAppService,
        IRoleAppService roleAppService,
        IUserMetaService userMetaService,
        IStringLocalizer<IdentityManagementResource> localizer)
    {
        _userListAppService = userListAppService;
        _identityUserAppService = identityUserAppService;
        _roleAppService = roleAppService;
        _userMetaService = userMetaService;
        _localizer = localizer;
    }

    [HttpGet]
    [Route("")]
    [Route("[action]")]
    public async Task<IActionResult> Index([FromQuery] UserFilterDto? filter, CancellationToken cancellationToken = default)
    {
        var f = filter ?? new UserFilterDto();
        var result = await _userListAppService.GetPaginatedAsync(f, cancellationToken);
        if (!result.IsSuccess)
            return View("Error", _localizer["Error:Message", result.ErrorMessage ?? ""].Value);
        ViewBag.Filter = f;
        return View(result.Result);
    }

    [HttpGet]
    [Route("[action]/{id:guid}")]
    public async Task<IActionResult> Manage(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = BonUserId.NewId(id);
        var infoResult = await _identityUserAppService.GetUserManageInfoAsync(userId);
        if (!infoResult.IsSuccess || infoResult.Result == null)
            return View("Error", _localizer["Users:NotFound"].Value);
        ViewBag.UserId = id;
        ViewBag.UserInfo = infoResult.Result;
        return View(infoResult.Result);
    }

    [HttpGet]
    [Route("[action]/{id:guid}")]
    public async Task<IActionResult> GetTabRoles(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = BonUserId.NewId(id);
        var rolesResult = await _identityUserAppService.GetUserRolesAsync(userId);
        if (!rolesResult.IsSuccess)
            return PartialView("_TabRoles", new ManageTabRolesViewModel { UserId = id, UserRoles = new List<UserRoleDto>(), AllRoles = new List<RoleDto>(), Error = rolesResult.ErrorMessage });
        var allRolesResult = await _roleAppService.PaginatedAsync(new RoleFilterDto { Take = 500, Skip = 0 });
        var allRoles = allRolesResult.Result?.Results?.ToList() ?? new List<RoleDto>();
        var model = new ManageTabRolesViewModel { UserId = id, UserRoles = rolesResult.Result ?? new List<UserRoleDto>(), AllRoles = allRoles };
        return PartialView("_TabRoles", model);
    }

    [HttpGet]
    [Route("[action]/{id:guid}")]
    public async Task<IActionResult> GetTabTokens(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = BonUserId.NewId(id);
        var result = await _identityUserAppService.GetUserTokensAsync(userId);
        if (!result.IsSuccess)
            return PartialView("_TabTokens", new ManageTabTokensViewModel { UserId = id, Tokens = new List<UserTokenDto>(), Error = result.ErrorMessage });
        return PartialView("_TabTokens", new ManageTabTokensViewModel { UserId = id, Tokens = result.Result ?? new List<UserTokenDto>() });
    }

    [HttpGet]
    [Route("[action]/{id:guid}")]
    public async Task<IActionResult> GetTabClaims(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = BonUserId.NewId(id);
        var result = await _identityUserAppService.GetUserClaimsAsync(userId);
        if (!result.IsSuccess)
            return PartialView("_TabClaims", new ManageTabClaimsViewModel { UserId = id, Claims = new List<UserClaimDto>(), Error = result.ErrorMessage });
        return PartialView("_TabClaims", new ManageTabClaimsViewModel { UserId = id, Claims = result.Result ?? new List<UserClaimDto>() });
    }

    [HttpGet]
    [Route("[action]/{id:guid}")]
    public async Task<IActionResult> GetTabMetas(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = BonUserId.NewId(id);
        var metas = await _userMetaService.GetAllAsync(userId, cancellationToken);
        var list = metas.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)).ToList();
        var model = new ManageTabMetasViewModel { UserId = id, Metas = list };
        return PartialView("_TabMetas", model);
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles([FromForm] Guid userId, [FromForm] List<string>? roleIds, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        var selectedIds = roleIds?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
        var currentResult = await _identityUserAppService.GetUserRolesAsync(user);
        if (currentResult.IsSuccess && currentResult.Result != null)
        {
            foreach (var ur in currentResult.Result.Where(ur => ur.Id?.Value != null && !selectedIds.Contains(ur.Id.Value)))
            {
                await _identityUserAppService.RemoveRoleAsync(new RemoveRoleInputDto { UserId = user, RoleId = ur.Id });
            }
        }
        if (selectedIds.Count > 0)
        {
            var roleIdList = selectedIds.Select(BonRoleId.NewId).ToList();
            var result = await _identityUserAppService.AssignRolesAsync(new AssignRolesInputDto { UserId = user, RoleIds = roleIdList });
            if (!result.IsSuccess)
            {
                TempData["Error"] = _localizer["Users:AssignRolesFailed", result.ErrorMessage ?? ""].Value;
                return RedirectToAction(nameof(Manage), new { id = userId });
            }
        }
        TempData["Success"] = _localizer["Users:RolesUpdatedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole([FromForm] Guid userId, [FromForm] string roleId, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        var role = BonRoleId.NewId(roleId);
        var result = await _identityUserAppService.RemoveRoleAsync(new RemoveRoleInputDto { UserId = user, RoleId = role });
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Users:AssignRolesFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Users:RoleRemovedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClaim([FromForm] Guid userId, [FromForm] string claimType, [FromForm] string claimValue, [FromForm] string? issuer, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        var result = await _identityUserAppService.AddUserClaimAsync(user, claimType ?? "", claimValue ?? "", issuer);
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Users:ClaimAddFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Users:ClaimAddedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveClaim([FromForm] Guid userId, [FromForm] string claimType, [FromForm] string claimValue, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        var result = await _identityUserAppService.RemoveUserClaimAsync(user, claimType ?? "", claimValue ?? "");
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Users:ClaimRemoveFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Users:ClaimRemovedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveToken([FromForm] Guid userId, [FromForm] string tokenType, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        var result = await _identityUserAppService.RemoveUserTokenAsync(user, tokenType ?? "");
        if (!result.IsSuccess)
            TempData["Error"] = _localizer["Users:TokenRemoveFailed", result.ErrorMessage ?? ""].Value;
        else
            TempData["Success"] = _localizer["Users:TokenRemovedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetMeta([FromForm] Guid userId, [FromForm] string metaKey, [FromForm] string metaValue, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        await _userMetaService.SetAsync(user, metaKey ?? "", metaValue ?? "", cancellationToken);
        TempData["Success"] = _localizer["Users:MetaSavedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }

    [HttpPost]
    [Route("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMeta([FromForm] Guid userId, [FromForm] string metaKey, CancellationToken cancellationToken = default)
    {
        var user = BonUserId.NewId(userId);
        await _userMetaService.DeleteAsync(user, metaKey ?? "", cancellationToken);
        TempData["Success"] = _localizer["Users:MetaDeletedSuccess"].Value;
        return RedirectToAction(nameof(Manage), new { id = userId });
    }
}
