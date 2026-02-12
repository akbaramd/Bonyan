using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Roles;

/// <summary>
/// Role metadata (key-value). Entity of the Role aggregate. Extensible data for roles.
/// </summary>
public class BonIdentityRoleMeta : BonEntity
{
    public BonRoleId RoleId { get; private set; } = null!;
    public string MetaKey { get; private set; } = string.Empty;
    public string MetaValue { get; private set; } = string.Empty;

    protected BonIdentityRoleMeta() { }

    public BonIdentityRoleMeta(BonRoleId roleId, string metaKey, string metaValue)
    {
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
        MetaKey = metaKey ?? throw new ArgumentNullException(nameof(metaKey));
        MetaValue = metaValue ?? throw new ArgumentNullException(nameof(metaValue));
    }

    public void UpdateValue(string value) => MetaValue = value ?? throw new ArgumentNullException(nameof(value));

    public override object GetKey() => new { RoleId, MetaKey };
}
