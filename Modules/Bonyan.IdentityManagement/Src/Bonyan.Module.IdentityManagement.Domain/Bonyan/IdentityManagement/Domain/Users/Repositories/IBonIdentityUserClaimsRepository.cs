using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserClaimsRepository<TUser,TRole> : IBonRepository<BonIdentityUserClaims<TUser,TRole>>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
}

public interface IBonIdentityUserClaimsReadOnlyRepository<TUser,TRole> : IBonReadOnlyRepository<BonIdentityUserClaims<TUser,TRole>>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
} 