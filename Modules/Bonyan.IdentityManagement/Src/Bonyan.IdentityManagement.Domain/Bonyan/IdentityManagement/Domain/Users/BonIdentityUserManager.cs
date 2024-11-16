using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bonyan.IdentityManagement.Domain.Abstractions.Users;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.DomainServices;

namespace Bonyan.IdentityManagement.Domain.Users
{
    public class BonIdentityUserManager<TUser> : BonUserManager<TUser>, IBonIdentityUserManager<TUser>
        where TUser : class, IBonIdentityUser
    {
        // Mocked role repository or source for demonstration; replace with actual data source.
        private readonly Dictionary<string, List<string>> _userRoles = new();

        public async Task<BonDomainResult> AssignRoleAsync(TUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            // Simulating asynchronous operation
            await Task.Run(() =>
            {
                if (!_userRoles.ContainsKey(user.Id.Value.ToString()))
                {
                    _userRoles[user.Id.Value.ToString()] = new List<string>();
                }

                if (!_userRoles[user.Id.Value.ToString()].Contains(roleName))
                {
                    _userRoles[user.Id.Value.ToString()].Add(roleName);
                }
            });
            return BonDomainResult.Success();;
        }

        public async Task<BonDomainResult> RemoveRoleAsync(TUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            // Simulating asynchronous operation
            await Task.Run(() =>
            {
                if (_userRoles.ContainsKey(user.Id.Value.ToString()))
                {
                    _userRoles[user.Id.Value.ToString()].Remove(roleName);
                }
            });
            return BonDomainResult.Success();;
        }

        public async Task<BonDomainResult<IReadOnlyList<string>>> GetUserRolesAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return BonDomainResult<IReadOnlyList<string>>.Success(_userRoles.ContainsKey(user.Id.Value.ToString())
                ? _userRoles[user.Id.Value.ToString()].AsReadOnly()
                : new List<string>().AsReadOnly());
        }
    }

    public class BonIdentityUserManager : BonIdentityUserManager<BonIdentityUser>, IBonIdentityUserManager
    {
        // Inherits all methods from the generic version.
    }
}