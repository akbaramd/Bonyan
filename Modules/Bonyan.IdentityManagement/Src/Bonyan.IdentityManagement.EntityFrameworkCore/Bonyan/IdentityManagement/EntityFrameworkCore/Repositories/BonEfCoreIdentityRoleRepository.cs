using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class
    BonIdentityEfCoreRoleRepository<TUser> :
    EfCoreBonRepository<BonIdentityRole, BonRoleId, IBonIdentityManagementDbContext<TUser>>,
    IBonIdentityRoleRepository, IBonIdentityRoleReadOnlyRepository where TUser : class, IBonIdentityUser
{
    protected override IQueryable<BonIdentityRole> PrepareQuery(DbSet<BonIdentityRole> dbSet)
    {
        return base.PrepareQuery(dbSet);
    }
}