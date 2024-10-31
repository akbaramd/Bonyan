using System.Collections.Generic;
using System.Threading.Tasks;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Bonyan.UserManagement.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing user entities with user-specific data access methods.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonyanUser"/>.</typeparam>
    public interface IBonyanUserRepository<TUser> :IUserStore<TUser>, IUserPasswordStore<TUser>, IRepository<TUser> where TUser : BonyanUser
    {
        /// <summary>
        /// Retrieves a user by their unique username.
        /// </summary>
        Task<TUser?> GetUserByUsernameAsync(string userName);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        Task<TUser?> GetUserByEmailAsync(Email email);

        /// <summary>
        /// Retrieves a user by their phone number.
        /// </summary>
        Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber);

        /// <summary>
        /// Retrieves all users with a specified status.
        /// </summary>
        Task<IReadOnlyList<TUser>> GetUsersByStatusAsync(UserStatus status);

        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        Task<TUser?> GetByIdAsync(UserId userId);

        /// <summary>
        /// Updates the information of an existing user.
        /// </summary>
        Task UpdateUserAsync(TUser user);

        /// <summary>
        /// Removes a user from the repository.
        /// </summary>
        Task DeleteUserAsync(TUser user);
    }
}