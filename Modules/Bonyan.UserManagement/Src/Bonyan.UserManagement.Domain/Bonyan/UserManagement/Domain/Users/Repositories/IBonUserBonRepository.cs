using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.Repositories
{
    public interface IBonUserRepository : IBonUserRepository<BonUser> 
    {

    }
    public interface IBonUserReadOnlyRepository : IBonUserReadOnlyRepository<BonUser> 
    {
     
    }
    

}