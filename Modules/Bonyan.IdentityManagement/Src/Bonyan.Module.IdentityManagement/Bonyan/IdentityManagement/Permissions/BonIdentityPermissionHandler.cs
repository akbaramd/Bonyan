using System.Security.Claims;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Permissions
{
    /// <summary>
    /// Identity-specific permission handler for authorization
    /// </summary>
    public class BonIdentityPermissionHandler : AuthorizationHandler<BonPermissionRequirement>
    {
        private readonly IBonPermissionManager _permissionManager;
        private readonly IBonCurrentUser _currentUser;
        private readonly ILogger<BonIdentityPermissionHandler> _logger;

        public BonIdentityPermissionHandler(
            IBonPermissionManager permissionManager,
            IBonCurrentUser currentUser,
            ILogger<BonIdentityPermissionHandler> logger)
        {
            _permissionManager = permissionManager;
            _currentUser = currentUser;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BonPermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogDebug("User is not authenticated, denying permission '{Permission}'", requirement.Permission);
                return;
            }

            try
            {
                var userId = _currentUser.Id;
                
                if (!userId.HasValue)
                {
                    _logger.LogWarning("User ID not found in claims, denying permission '{Permission}'", requirement.Permission);
                    return;
                }

                var requiredPermissions = requirement.GetAllPermissions().ToList();
                var hasPermissions = new List<bool>();

                foreach (var permission in requiredPermissions)
                {
                    var hasPermission = await CheckPermissionAsync(context.User, BonUserId.NewId(userId.Value), permission);
                    hasPermissions.Add(hasPermission);
                }

                var success = requirement.RequireAll 
                    ? hasPermissions.All(x => x) 
                    : hasPermissions.Any(x => x);

                if (success)
                {
                    _logger.LogDebug("User '{UserId}' granted permission '{Permission}'", userId, requirement.Permission);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogDebug("User '{UserId}' denied permission '{Permission}'", userId, requirement.Permission);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission '{Permission}' for user", requirement.Permission);
                // Don't succeed on errors - fail securely
            }
        }

        /// <summary>
        /// Checks if a user has a specific permission through direct claims or role claims
        /// </summary>
        private async Task<bool> CheckPermissionAsync(ClaimsPrincipal user, BonUserId userId, string permission)
        {
            try
            {
                // First check direct user claims
                if (HasDirectPermissionClaim(user, permission))
                {
                    _logger.LogDebug("User '{UserId}' has direct permission claim for '{Permission}'", userId, permission);
                    return true;
                }

                // Check role-based permissions
                if (await HasRoleBasedPermissionAsync(user, permission))
                {
                    _logger.LogDebug("User '{UserId}' has role-based permission for '{Permission}'", userId, permission);
                    return true;
                }

                // Check hierarchical permissions (if user has parent permission)
                if (await HasHierarchicalPermissionAsync(user, userId, permission))
                {
                    _logger.LogDebug("User '{UserId}' has hierarchical permission for '{Permission}'", userId, permission);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission '{Permission}' for user '{UserId}'", permission, userId);
                return false;
            }
        }

        /// <summary>
        /// Checks if user has direct permission claim
        /// </summary>
        private static bool HasDirectPermissionClaim(ClaimsPrincipal user, string permission)
        {
            return user.HasClaim(BonClaimTypes.Permission, permission);
        }

        /// <summary>
        /// Checks if user has role-based permission
        /// </summary>
        private async Task<bool> HasRoleBasedPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            foreach (var role in roles)
            {
                // Check if role has this permission
                if (user.HasClaim($"role.{role}.permission", permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if user has hierarchical permission (parent permission)
        /// </summary>
        private async Task<bool> HasHierarchicalPermissionAsync(ClaimsPrincipal user, BonUserId userId, string permission)
        {
            var permissionDefinition = _permissionManager.GetPermission(permission);
            
            if (permissionDefinition?.Parent != null)
            {
                var parentPermission = permissionDefinition.Parent.Name;
                return await CheckPermissionAsync(user, userId, parentPermission);
            }

            return false;
        }

        /// <summary>
        /// Gets the current user's ClaimsPrincipal
        /// </summary>
        private ClaimsPrincipal GetCurrentUserClaimsPrincipal()
        {
            // Create a ClaimsPrincipal from current user's claims
            var claims = _currentUser.GetAllClaims();
            var identity = new ClaimsIdentity(claims, "BonyanIdentity");
            return new ClaimsPrincipal(identity);
        }
    }
} 