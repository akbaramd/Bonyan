using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRole : IEntity<RoleId>
{
    public string Title { get;  }
    public string Name { get;  }

    public IReadOnlyCollection<BonPermission> Permissions { get; }
}