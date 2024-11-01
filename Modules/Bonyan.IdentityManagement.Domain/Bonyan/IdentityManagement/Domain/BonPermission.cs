using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public class BonPermission : Entity, IBonPermission
{
    protected BonPermission()
    {
    }

    public override object[] GetKeys()
    {
        return [Key];
    }

    public string Key { get; set; }
    public string Title { get; set; }
}