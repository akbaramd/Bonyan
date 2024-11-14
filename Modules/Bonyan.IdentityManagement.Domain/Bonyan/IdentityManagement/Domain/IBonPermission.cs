using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Entity;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonPermission : IBonEntity
{
    public string Key { get; set; }
    public string Title { get; set; }
}