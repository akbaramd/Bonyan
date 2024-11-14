using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRole : IBonEntity<BonRoleId>
{
    public string Title { get;  }
    public string Name { get;  }

    public IReadOnlyCollection<BonIdentityPermission> Permissions { get; }
}