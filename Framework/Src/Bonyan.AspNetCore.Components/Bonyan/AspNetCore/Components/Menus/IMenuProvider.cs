namespace Bonyan.AspNetCore.Components.Menus;

public interface IMenuProvider
{
    IEnumerable<MenuItem> GetMenuItems(string location);
}