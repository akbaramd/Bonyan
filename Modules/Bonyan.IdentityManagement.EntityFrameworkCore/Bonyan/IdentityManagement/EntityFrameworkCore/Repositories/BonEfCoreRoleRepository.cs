using Bonyan.IdentityManagement.Domain;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreRoleRepository : EfCoreBonRepository<BonIdentityRole,BonRoleId,BonIdentityManagementDbContext>,IBonRoleRepository,IBonRoleReadOnlyRepository
{
    public BonEfCoreRoleRepository(BonIdentityManagementDbContext userManagementDbContext) : base(userManagementDbContext)
    {
    }
}