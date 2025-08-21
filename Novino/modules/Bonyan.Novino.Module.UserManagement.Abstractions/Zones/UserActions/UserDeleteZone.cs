using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.UserActions;

/// <summary>
/// Zone model for user delete functionality
/// Contains only essential data - components should fetch their own data
/// Allows other modules to add custom fields and validation
/// </summary>
public class UserDeleteZone : IZone
{
    public string ZoneId => "user_delete";
    public string ZoneName => "UserManagementUserDelete";
    public IDictionary<string, object>? Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// User ID - the only essential data needed by delete components
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Add custom form field HTML to be rendered
    /// </summary>
    /// <param name="fieldHtml">The HTML for the form field</param>
    public void AddCustomField(string fieldHtml)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("CustomFields"))
            Metadata["CustomFields"] = new List<string>();

        ((List<string>)Metadata["CustomFields"]).Add(fieldHtml);
    }

    /// <summary>
    /// Add custom validation script
    /// </summary>
    /// <param name="validationScript">The JavaScript validation code</param>
    public void AddValidationScript(string validationScript)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("ValidationScripts"))
            Metadata["ValidationScripts"] = new List<string>();

        ((List<string>)Metadata["ValidationScripts"]).Add(validationScript);
    }

    /// <summary>
    /// Add additional data that can be used by components
    /// </summary>
    /// <param name="key">The key for the data</param>
    /// <param name="value">The value to store</param>
    public void AddAdditionalData(string key, object value)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("AdditionalData"))
            Metadata["AdditionalData"] = new Dictionary<string, object>();

        ((Dictionary<string, object>)Metadata["AdditionalData"])[key] = value;
    }
} 