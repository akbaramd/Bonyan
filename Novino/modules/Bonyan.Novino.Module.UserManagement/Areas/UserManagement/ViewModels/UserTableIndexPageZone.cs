using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.ViewModels;

/// <summary>
/// ViewModel for user table index page zone that combines user data with permissions
/// Allows other modules to add custom action buttons for each user row
/// </summary>
public class UserTableIndexPageZone : IZone
{
    /// <summary>
    /// The user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The username
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// The user email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the user is locked
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Whether the user can be viewed in detail
    /// </summary>
    public bool CanDetail { get; set; }

    /// <summary>
    /// Whether the user can be edited
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Whether the user can be deleted
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Whether the user password can be changed
    /// </summary>
    public bool CanChangePassword { get; set; }

    /// <summary>
    /// Whether the user password can be reset
    /// </summary>
    public bool CanResetPassword { get; set; }

    /// <summary>
    /// Whether the user can be locked
    /// </summary>
    public bool CanLock { get; set; }

    /// <summary>
    /// Whether the user can be unlocked
    /// </summary>
    public bool CanUnlock { get; set; }

    /// <summary>
    /// Whether the user can be activated
    /// </summary>
    public bool CanActivate { get; set; }

    /// <summary>
    /// Whether the user can be deactivated
    /// </summary>
    public bool CanDeactivate { get; set; }

    /// <summary>
    /// Whether the user roles can be managed
    /// </summary>
    public bool CanManageRoles { get; set; }

    /// <summary>
    /// Whether the user claims can be managed
    /// </summary>
    public bool CanManageClaims { get; set; }

    /// <summary>
    /// Whether data can be exported
    /// </summary>
    public bool CanExport { get; set; }

    /// <summary>
    /// Whether a message can be sent to the user
    /// </summary>
    public bool CanSendMessage { get; set; }

    /// <summary>
    /// Whether an email can be sent to the user
    /// </summary>
    public bool CanSendEmail { get; set; }

    /// <summary>
    /// Whether a welcome email can be sent to the user
    /// </summary>
    public bool CanSendWelcomeEmail { get; set; }

    /// <summary>
    /// Custom action buttons from other modules
    /// </summary>
    public List<string> CustomActions { get; set; } = new();

    /// <summary>
    /// Additional data that can be used by other modules
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Create from UserListViewModel and permission flags
    /// </summary>
    public static UserTableIndexPageZone FromUserListViewModel(UserListViewModel user, UserListIndexViewModel indexModel)
    {
        return new UserTableIndexPageZone
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            IsActive = user.IsActive,
            IsLocked = user.IsLocked,
            
            // Copy permissions from index model
            CanDetail = indexModel.CanDetails,
            CanEdit = indexModel.CanEdit,
            CanDelete = indexModel.CanDelete,
            CanChangePassword = indexModel.CanEdit,
            CanResetPassword = indexModel.CanEdit,
            CanLock = indexModel.CanEdit,
            CanUnlock = indexModel.CanEdit,
            CanActivate = indexModel.CanEdit,
            CanDeactivate = indexModel.CanEdit,
            CanManageRoles = indexModel.CanEdit,
            CanManageClaims = indexModel.CanEdit,
            CanExport = indexModel.CanExport,
            CanSendMessage = true,
            CanSendEmail = true,
            CanSendWelcomeEmail = true
        };
    }

    /// <summary>
    /// Add a custom action button
    /// </summary>
    /// <param name="actionHtml">The HTML for the action button</param>
    public void AddCustomAction(string actionHtml)
    {
        if (!string.IsNullOrWhiteSpace(actionHtml))
        {
            CustomActions.Add(actionHtml);
        }
    }

    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    public string ZoneId => "user-table-index-page-zone";

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    public string ZoneName => "user-table-actions";

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    public IDictionary<string, object>? Metadata => AdditionalData;
} 