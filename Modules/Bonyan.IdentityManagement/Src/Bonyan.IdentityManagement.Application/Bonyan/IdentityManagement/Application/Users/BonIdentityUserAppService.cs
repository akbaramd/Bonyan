using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;  // Add this
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application.Roles
{
    public class BonIdentityUserAppService<TUser> :
        BonCrudAppService<TUser, BonUserId, BonFilterAndPaginateDto, BonIdentityUserDto, BonIdentityUserDto, BonIdentityUserCreateDto, BonIdentityUserUpdateDto>,
        IBonIdentityUserAppService where TUser : class, IBonIdentityUser
    {
        // We need to use both UserManager and RoleManager
        public IBonIdentityUserManager<TUser> UserManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserManager<TUser>>();
        public IBonIdentityRoleManager RoleManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleManager>();  // New RoleManager

        // Create a new user with roles
        public override async Task<ServiceResult<BonIdentityUserDto>> CreateAsync(BonIdentityUserCreateDto input)
        {
            try
            {
                var user = MapCreateDtoToEntity(input);

                // Create the user with password
                var creationResult = await UserManager.CreateAsync(user, input.Password);
                if (!creationResult.IsSuccess)
                {
                    return ServiceResult<BonIdentityUserDto>.Failure(creationResult.ErrorMessage);
                }

             
                var roleAssignmentResult = await UserManager.AssignRolesAsync(user, input.Roles.Select(BonRoleId.NewId));

                return ServiceResult<BonIdentityUserDto>.Success(MapToDto(user));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error creating user.");
                return ServiceResult<BonIdentityUserDto>.Failure("Error creating user.");
            }
        }

        // Update an existing user and roles
        public override async Task<ServiceResult<BonIdentityUserDto>> UpdateAsync(BonUserId id, BonIdentityUserUpdateDto input)
        {
            try
            {
                var user = await Repository.FindOneAsync(x => x.Id == id);

                if (user == null)
                {
                    return ServiceResult<BonIdentityUserDto>.Failure($"User with ID {id.Value} not found.");
                }

                MapUpdateDtoToEntity(input, user);

         
                await UserManager.AssignRolesAsync(user,input.Roles.Select(BonRoleId.NewId));
                // Save changes
                var updateResult = await UserManager.UpdateAsync(user);
                if (!updateResult.IsSuccess)
                {
                    return ServiceResult<BonIdentityUserDto>.Failure(updateResult.ErrorMessage);
                }

                return ServiceResult<BonIdentityUserDto>.Success(MapToDto(user));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error updating user with ID {id.Value}.");
                return ServiceResult<BonIdentityUserDto>.Failure("Error updating user.");
            }
        }

        // Map create DTO to entity
    }
}
