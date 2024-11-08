using System.Reflection;
using System.Security.Principal;

namespace Bonyan.AspNetCore.Components;

public class BonBlazorOptions
{
    public Assembly AppAssembly { get; set; } = typeof(BonBlazorOptions).Assembly;

    public Assembly[] AdditionalAssembly { get; set; } = [];
}