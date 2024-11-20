using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application;

public interface IBonIdentityAuthService : IBonApplicationService
{
    Task<ServiceResult<bool>> RegisterAsync(BonIdentityUserRegistererDto createDto);
    Task<ServiceResult<BonIdentityUserDto>> ProfileAsync();
    Task<ServiceResult<bool>> CookieSignInAsync(string username, string password, bool isPersistent);
    Task<ServiceResult<BonIdentityJwtResultDto>> JwtBearerSignInAsync(string username, string password);
    Task<ServiceResult<BonIdentityJwtResultDto>> RefreshTokenAsync(string refreshToken);
 
}