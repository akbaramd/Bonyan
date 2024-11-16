using System.ComponentModel.DataAnnotations;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Permissions;

public class BonIdentityPermission : BonEntity
{
    protected BonIdentityPermission()
    {
    }

    public override object[] GetKeys()
    {
        return [Key];
    }

    [Key] public string Key { get; set; } = default!;
    public string Title { get; set; } = default!;
}