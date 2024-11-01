using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonPermission : IEntity
{
    public string Key { get; set; }
    public string Title { get; set; }
}