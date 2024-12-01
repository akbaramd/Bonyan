using System.Text;
using Bonyan.AspNetCore.Authentication.Options;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.AspNetCore.Authentication
{
    public class BonAuthorizationConfiguration
    {
        private readonly BonConfigurationContext _context;
        
        public BonAuthorizationConfiguration(BonConfigurationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public  void RegisterPermissions( string[] permissions)
        {
            // Register PermissionAccessor as a singleton
            _context.Services.AddObjectAccessor<PermissionAccessor>(new PermissionAccessor(permissions));

            
        }
    }
}