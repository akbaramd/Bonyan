using System.Collections.Generic;
using System.Threading.Tasks;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing user entities with user-specific data access methods.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonyanUser"/>.</typeparam>
    public interface IBonyanUserRepository<TUser> : IRepository<TUser> where TUser : BonyanUser
    {
        Task<TUser?> GetUserByUsernameAsync(string userName);

        Task<TUser?> GetUserByEmailAsync(Email email);

        Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber);



    }
}