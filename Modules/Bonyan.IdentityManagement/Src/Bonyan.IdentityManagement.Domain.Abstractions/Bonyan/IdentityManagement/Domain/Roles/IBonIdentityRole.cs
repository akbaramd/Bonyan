using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityRole : IBonAggregateRoot<BonRoleId>
{
    public string Title { get;  } 
    public string Name { get;  } 
}