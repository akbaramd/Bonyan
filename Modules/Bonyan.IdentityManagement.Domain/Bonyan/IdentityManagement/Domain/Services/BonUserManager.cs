using Bonyan.Layer.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bonyan.UserManagement.Domain.Services
{
    /// <summary>
    /// Domain service responsible for managing user states and actions.
    /// Provides methods to update profiles, manage password changes, handle verification, and manage account status.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonyanUser"/>.</typeparam>
    public class BonUserManager<TUser>(
        IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<TUser>> logger)
        : UserManager<TUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
            keyNormalizer, errors, services, logger),IDomainService
        where TUser : BonyanUser
    {
    }
}
