using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserClaimsRepository<TUser, TRole> : EfCoreBonRepository<BonIdentityUserClaims<TUser, TRole>, BonUserClaimId, IBonIdentityManagementDbContext<TUser, TRole>>, IBonIdentityUserClaimsRepository<TUser, TRole> 
    where TUser : BonIdentityUser<TUser, TRole>
    where TRole : BonIdentityRole<TRole>
{
}

public class BonEfCoreIdentityUserClaimsReadOnlyRepository<TUser, TRole> : EfCoreReadonlyRepository<BonIdentityUserClaims<TUser, TRole>, BonUserClaimId, IBonIdentityManagementDbContext<TUser, TRole>>, IBonIdentityUserClaimsReadOnlyRepository<TUser, TRole> 
    where TUser : BonIdentityUser<TUser, TRole>
    where TRole : BonIdentityRole<TRole>
{
    
} 