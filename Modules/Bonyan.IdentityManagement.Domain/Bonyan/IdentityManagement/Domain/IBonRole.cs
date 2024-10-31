using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRole : IEntity<RoleId>
{
    public string Title { get; set; }
    public string Name { get; set; }
}

public class RoleId : BusinessId<RoleId>{}