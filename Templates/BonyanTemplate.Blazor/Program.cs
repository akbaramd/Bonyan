using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Components.Admin;
using Bonyan.AspNetCore.Components.Admin.Components;
using BonyanTemplate.Blazor;
using BonyanTemplate.Blazor.Components;

var builder = BonyanApplication.CreateApplicationBuilder<BonyanTemplateBlazorModule>(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = await builder.BuildAsync();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


var themeService = app.Services.GetRequiredService<ThemeService>();
var menuService = themeService.MenuService;

// Define a menu
var mainMenu = new Menu
{
    Name = "MainMenu",
    Items = new List<MenuItem>
    {
        new MenuItem
        {
            Title = "صفحه نخست",
            Url = "/dashboard",
            Icon = "nav-icon fas fa-home",
            MetaData = new Dictionary<string, string>
            {
                { "customKey", "customValue" }
            }
        },
        new MenuItem
        {
            Title = "مدیریت کاربران",
            Url = "#",
            Icon = "nav-icon fas fa-users",
            Children = new List<MenuItem>
            {
                new MenuItem
                {
                    Title = "لیست کاربران",
                    Url = "/users",
                    Icon = "far fa-circle nav-icon"
                },
                new MenuItem
                {
                    Title = "افزودن کاربر",
                    Url = "/users/add",
                    Icon = "far fa-circle nav-icon"
                }
            }
        },
        // Add more menu items as needed
    }
};

// Add the menu to the MenuService
menuService.AddMenu(mainMenu.Name, mainMenu);

// Assign the menu to the sidebar location
menuService.AssignMenuToLocation("sidebar", mainMenu.Name);

app.Run();