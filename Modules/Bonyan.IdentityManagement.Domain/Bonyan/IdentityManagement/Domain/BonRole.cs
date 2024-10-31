using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain;

public class BonRole : Entity<RoleId>, IBonRole
{
    protected BonRole(){}
    
    public BonRole(string title, string name)
    {
        Title = title;
        Name = name;
    }

    public string Title { get; set; }
    public string Name { get; set; }
}