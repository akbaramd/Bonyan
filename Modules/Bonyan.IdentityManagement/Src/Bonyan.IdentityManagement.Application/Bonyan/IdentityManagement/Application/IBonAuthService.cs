using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application;

public interface IBonAuthService : IBonApplicationService
{
    Task<ServiceResult<bool>> CookieSignInAsync(string username, string password,bool isPersistent);
    Task<ServiceResult<JwtResultDto>> JwtBearerSignInAsync(string username, string password);
}