using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.UserManagement.Domain
{
    /// <summary>
    /// Domain service responsible for managing user states and actions.
    /// Provides methods to update profiles, manage password changes, handle verification, and manage account status.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonyanUser"/>.</typeparam>
    public class BonyanUserManager<TUser>: UserManager<BonyanUser>  where TUser : BonyanUser
    {
        public BonyanUserManager(IUserStore<BonyanUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<BonyanUser> passwordHasher, IEnumerable<IUserValidator<BonyanUser>> userValidators, IEnumerable<IPasswordValidator<BonyanUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<BonyanUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
