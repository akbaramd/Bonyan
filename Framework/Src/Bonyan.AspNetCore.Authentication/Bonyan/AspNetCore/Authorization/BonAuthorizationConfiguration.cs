using Bonyan.AspNetCore.Authorization.Permissions;
using Bonyan.Modularity;

namespace Bonyan.AspNetCore.Authorization
{
    public class BonAuthorizationConfiguration
    {
        private readonly BonConfigurationContext _context;
        
        public BonAuthorizationConfiguration(BonConfigurationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.Services.AddObjectAccessor(new PermissionAccessor());
        }

        public  void RegisterPermissions( string[] permissions)
        {
            // Register PermissionAccessor as a singleton
            _context.Services.GetObject<PermissionAccessor>().AddRange(permissions);

            
        }
    }
}