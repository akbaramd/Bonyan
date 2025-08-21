using System;

namespace Bonyan.Ui.Novino.Core;

/// <summary>
/// Attribute to mark ViewComponents with their zone and order
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ZoneAttribute : Attribute
{
    public ZoneAttribute(string zone, int order = 0)
    {
        Zone = zone;
        Order = order;
    }
    
    public string Zone { get; }
    public int Order { get; }
} 