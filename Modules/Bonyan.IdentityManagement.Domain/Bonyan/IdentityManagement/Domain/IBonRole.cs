using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Entity;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRole : IBonEntity<BonRoleId>
{
    public string Title { get;  }
    public string Name { get;  }

    public IReadOnlyCollection<BonIdentityPermission> Permissions { get; }
}