using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users.DomainServices;

public interface IBonUserManager<TUser> : IBonDomainService where TUser : IBonUser
{
    Task<BonDomainResult> CreateAsync(TUser entity);
    
    Task<BonDomainResult> UpdateAsync(TUser entity);
    Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName);
  
}