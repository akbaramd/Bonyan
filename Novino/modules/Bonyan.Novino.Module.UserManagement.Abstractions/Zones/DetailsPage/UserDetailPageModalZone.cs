using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.DetailsPage;

/// <summary>
/// Zone model for user detail page modal
/// Contains only essential data - components should fetch their own data
/// </summary>
public class UserDetailPageModalZone : IZone
{
    public string ZoneId => "user_detail_page_modal";
    public string ZoneName => "UserManagementUserDetailPageModal";
    public IDictionary<string, object>? Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// User ID - the only essential data needed by modal components
    /// </summary>
    public string UserId { get; set; } = string.Empty;
} 