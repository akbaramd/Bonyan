using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.Repositories
{
    /// <summary>
    /// Repository interface for managing user entities with user-specific data access methods.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonUserEntity"/>.</typeparam>
    public interface IBonUserRepository<TUser> : IBonRepository<TUser> where TUser : class, IBonUser
    {
        Task<TUser?> GetUserByUsernameAsync(string userName);

        Task<TUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail);

        Task<TUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber);
    }

    public interface IBonUserReadOnlyRepository<TUser> : IBonReadOnlyRepository<TUser>
        where TUser : class, IBonUser
    {
        Task<TUser?> GetUserByUsernameAsync(string userName);

        Task<TUser?> GetUserByEmailAsync(BonUserEmail bonUserEmail);

        Task<TUser?> GetUserByPhoneNumberAsync(BonUserPhoneNumber bonUserPhoneNumber);
    }
    
}