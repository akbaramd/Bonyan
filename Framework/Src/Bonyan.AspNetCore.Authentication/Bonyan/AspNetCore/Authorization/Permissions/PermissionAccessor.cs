namespace Bonyan.IdentityManagement.Permissions;

public class PermissionAccessor : List<string>
{
    public PermissionAccessor(string[] permissions)
    {
        this.AddRange(permissions);
    }
}