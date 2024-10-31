
using Bonyan.UserManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class IdentityManagementEntityTypeBuilderExtensions
{
    public static ModelBuilder ConfigureIdentityManagementByConvention<TUser>(this ModelBuilder modelBuilder) where TUser: BonyanUser
    {
        modelBuilder.ConfigureUserManagementByConvention<TUser>();
        
    
        return modelBuilder;
    }
}