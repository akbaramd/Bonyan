﻿using System.Reflection;

namespace Bonyan.AspNetCore.Components;

public class BonBlazorOptions
{
    public Assembly AppAssembly { get; set; } 

    public List<Assembly> AdditionalAssembly { get;  } = [];


    public void AddAssembly<TType>()
    {
        AdditionalAssembly.Add(typeof(TType).Assembly);
    }
}