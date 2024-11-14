using Bonyan.Layer.Domain.Repository;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing user entities with user-specific data access methods.
    /// </summary>
    /// <typeparam name="TUser">The user type implementing <see cref="BonUser"/>.</typeparam>
    public interface IBonUserRepository<TUser> : IBonRepository<TUser> where TUser : BonUser
    {
        Task<TUser?> GetUserByUsernameAsync(string userName);

        Task<TUser?> GetUserByEmailAsync(Email email);

        Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber);
    }
    public interface IBonUserReadOnlyRepository<TUser> : IBonReadOnlyRepository<TUser> where TUser : BonUser
    {
        Task<TUser?> GetUserByUsernameAsync(string userName);

        Task<TUser?> GetUserByEmailAsync(Email email);

        Task<TUser?> GetUserByPhoneNumberAsync(PhoneNumber phoneNumber);
    }
    
    
    public interface IBonUserRepository : IBonUserRepository<BonUser> 
    {

    }
    public interface IBonUserReadOnlyRepository : IBonUserReadOnlyRepository<BonUser> 
    {
     
    }
}