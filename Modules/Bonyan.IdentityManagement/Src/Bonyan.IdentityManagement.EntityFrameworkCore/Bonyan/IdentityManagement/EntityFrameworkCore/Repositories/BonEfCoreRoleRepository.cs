using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreRoleRepository : EfCoreBonRepository<BonIdentityRole,BonRoleId,BonIdentityManagementDbContext>,IBonRoleRepository,IBonRoleReadOnlyRepository
{
    public BonEfCoreRoleRepository(BonIdentityManagementDbContext userManagementDbContext) : base(userManagementDbContext)
    {
    }
}