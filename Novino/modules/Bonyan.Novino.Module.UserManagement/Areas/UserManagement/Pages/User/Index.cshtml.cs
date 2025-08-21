using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;
using Bonyan.UnitOfWork;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Pages.User
{
    /// <summary>
    /// Page model for user list with filtering and pagination
    /// </summary>
    [Authorize(Policy = UserManagementPermissions.Users.View)]
    [Area("UserManagement")]
    public class IndexModel : PageModel
    {
        private readonly IBonIdentityUserManager<Domain.Entities.User, Role> _userManager;
        private readonly IBonIdentityUserRepository<Domain.Entities.User, Role> _userRepository;
        private readonly IBonIdentityUserReadOnlyRepository<Domain.Entities.User, Role> _userReadOnlyRepository;
        private readonly IBonIdentityRoleRepository<Role> _roleRepository;
        private readonly IBonIdentityRoleManager<Role> _roleManager;
        private readonly IBonPermissionManager<Domain.Entities.User, Role> _permissionManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IBonCurrentUser _currentUser;
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;

        public IndexModel(
            IBonIdentityUserManager<Domain.Entities.User, Role> userManager,
            IBonIdentityRoleManager<Role> roleManager,
            IBonPermissionManager<Domain.Entities.User, Role> permissionManager,
            ILogger<IndexModel> logger,
            IBonCurrentUser currentUser, 
            IBonIdentityUserRepository<Domain.Entities.User, Role> userRepository, 
            IBonIdentityRoleRepository<Role> roleRepository, 
            IBonIdentityUserReadOnlyRepository<Domain.Entities.User, Role> userReadOnlyRepository, 
            IBonUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userReadOnlyRepository = userReadOnlyRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "UserName";

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc";

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public UserListIndexViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Validate pagination parameters
                Page = Math.Max(1, Page);
                PageSize = Math.Max(1, Math.Min(100, PageSize)); // Limit page size to 100
                SortOrder = SortOrder?.ToLower() == "desc" ? "desc" : "asc";

                // Use readonly repository for querying and pagination
                var usersQuery = await _userReadOnlyRepository.GetQueryableAsync();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    var search = SearchTerm.Trim();
                    usersQuery = usersQuery.Where(u =>
                        u.UserName.Contains(search) ||
                        (u.Email != null && u.Email.Address.Contains(search)) ||
                        u.Profile.FirstName.Contains(search) ||
                        u.Profile.LastName.Contains(search) ||
                        (u.PhoneNumber != null && u.PhoneNumber.Number.Contains(search))
                    );
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(Status))
                {
                    switch (Status.ToLower())
                    {
                        case "active":
                            usersQuery = usersQuery.Where(u => u.Status == UserStatus.Active);
                            break;
                        case "inactive":
                            usersQuery = usersQuery.Where(u => u.Status == UserStatus.Inactive);
                            break;
                        case "locked":
                            usersQuery = usersQuery.Where(u => u.Status == UserStatus.Locked);
                            break;
                    }
                }

                // Apply sorting
                usersQuery = SortBy?.ToLower() switch
                {
                    "email" => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.Email != null ? u.Email.Address : "") : usersQuery.OrderBy(u => u.Email != null ? u.Email.Address : ""),
                    "name" => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.Profile.FirstName) : usersQuery.OrderBy(u => u.Profile.FirstName),
                    "surname" => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.Profile.LastName) : usersQuery.OrderBy(u => u.Profile.LastName),
                    "status" => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.Status) : usersQuery.OrderBy(u => u.Status),
                    "createdat" => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.CreatedAt) : usersQuery.OrderBy(u => u.CreatedAt),
                    _ => SortOrder == "desc" ? usersQuery.OrderByDescending(u => u.UserName) : usersQuery.OrderBy(u => u.UserName)
                };

                // Get total count for pagination using readonly repository
                var totalCount = await usersQuery.CountAsync();

                // Apply pagination using readonly repository
                var users = await usersQuery
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                // Get user roles for each user
                var userList = new List<UserListViewModel>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetUserRolesAsync(user);
                    var isLocked = user.Status == UserStatus.Locked;

                    userList.Add(new UserListViewModel
                    {
                        Id = user.Id.ToString(),
                        UserName = user.UserName,
                        Email = user.Email?.Address,
                        Name = user.Profile.FirstName,
                        SurName = user.Profile.LastName,
                        PhoneNumber = user.PhoneNumber?.Number,
                        Roles = roles.Value.Select(x => x.Title).ToList(),
                        IsActive = user.Status == UserStatus.Active,
                        IsLocked = isLocked,
                        CreatedAt = user.CreatedAt,
                        EmailConfirmed = user.Email != null && user.Email.IsVerified,
                        PhoneNumberConfirmed = user.PhoneNumber != null && user.PhoneNumber.IsVerified
                    });
                }

                // Create pagination info
                var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
                var hasPreviousPage = Page > 1;
                var hasNextPage = Page < totalPages;

                // Get available roles for filter dropdown using readonly repository
                var availableRoles = (await _roleRepository.FindAsync(x => true)).Select(r => r.Title).ToList();

                ViewModel = new UserListIndexViewModel
                {
                    Users = userList,
                    Filter = new UserFilterModel
                    {
                        SearchTerm = SearchTerm,
                        SortBy = SortBy ?? "UserName",
                        SortOrder = SortOrder ?? "asc",
                        Status = Status,
                        Page = Page,
                        PageSize = PageSize
                    },
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPreviousPage = hasPreviousPage,
                    HasNextPage = hasNextPage,
                    AvailableRoles = availableRoles,
                    CanCreate = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.Create),
                    CanDetails = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.Details),
                    CanEdit = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.Edit),
                    CanDelete = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.Delete),
                    CanExport = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.Export),
                    CanBulkDelete = await _permissionManager.HasPermissionAsync(BonUserId.FromValue(_currentUser.GetId()), UserManagementPermissions.Users.BulkDelete)
                };

                ViewData["Title"] = "مدیریت کاربران";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading user list");
                TempData["ErrorMessage"] = "خطایی در بارگذاری لیست کاربران رخ داد.";
                return RedirectToPage("/Index", new { area = "" });
            }
        }
    }
} 