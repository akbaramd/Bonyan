using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.DetailsPage;

/// <summary>
/// Zone model for user detail page header
/// Contains only essential data - components should fetch their own data
/// </summary>
public class UserDetailPageHeaderZone : IZone
{
    public string ZoneId => "user_detail_page_header";
    public string ZoneName => "UserManagementUserDetailPageHeader";
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// User ID - the only essential data needed by header components
    /// </summary>
    public string UserId { get; set; } = string.Empty;
} 