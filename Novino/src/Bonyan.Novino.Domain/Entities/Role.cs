using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;

namespace Bonyan.Novino.Domain.Entities;

public class Role : BonIdentityRole<Role>
{
    public Role(BonRoleId id, string title, bool canBeDeleted = true):base(id,title,canBeDeleted)
    {
      
    }
} 