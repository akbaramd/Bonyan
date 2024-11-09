namespace Bonyan.AspNetCore.Components.Menus
{
    public class Menu
    {
        public string Name { get; set; }
        public List<MenuItem> Items { get; set; } = new();
    }
    
    public class MenuItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public Dictionary<string, string> MetaData { get; set; } = new();
        public List<MenuItem> Children { get; set; } = new();
        public int Order { get; set; } = 0; // Default order is 0
    }
}

