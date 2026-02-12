using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Application.Users;

/// <summary>
/// Application service for paginated user list (read-only). Used by Users index.
/// </summary>
public interface IIdentityUserListAppService
{
    Task<ServiceResult<BonPaginatedResult<UserListDto>>> GetPaginatedAsync(UserFilterDto filter, CancellationToken cancellationToken = default);
}
