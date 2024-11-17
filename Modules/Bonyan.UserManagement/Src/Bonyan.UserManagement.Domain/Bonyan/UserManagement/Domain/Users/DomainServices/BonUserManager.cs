using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.Repositories;
using Microsoft.Extensions.Logging;

namespace Bonyan.UserManagement.Domain.Users.DomainServices;

public class BonUserManager<TUser> : BonDomainService, IBonUserManager<TUser> where TUser : class, IBonUser
{
    public IBonUserRepository<TUser> UserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonUserRepository<TUser>>();

    // Create a new user and set an initial password
   

    // Update user information
    public async Task<BonDomainResult> UpdateAsync(TUser entity)
    {
        try
        {
            await UserRepository.UpdateAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating user.");
            return BonDomainResult.Failure("Error updating user.");
        }
    }

    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.UserName.Equals(userName));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with username {userName} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<TUser>.Failure("Error finding user by username.");
        }
    }
    public async Task<BonDomainResult> CreateAsync(TUser entity)
    {
        try
        {
            if (await UserRepository.ExistsAsync(x => x.UserName.Equals(entity.UserName)))
            {
                Logger.LogWarning($"User with username {entity.UserName} already exists.");
                return BonDomainResult.Failure($"User with username {entity.UserName} already exists.");
            }
            
            await UserRepository.AddAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult.Failure("Error creating user.");
        }
    }
    // Change a user's password

}