using System.ComponentModel.DataAnnotations;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public class BonIdentityPermission : BonEntity, IBonPermission
{
    protected BonIdentityPermission()
    {
    }

    public override object[] GetKeys()
    {
        return [Key];
    }

    [Key]
    public string Key { get; set; }
    public string Title { get; set; }
}