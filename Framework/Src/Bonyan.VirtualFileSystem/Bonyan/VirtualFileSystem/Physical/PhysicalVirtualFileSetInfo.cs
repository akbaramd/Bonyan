﻿using Bonyan.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;

namespace Bonyan.VirtualFileSystem.Physical;

public class PhysicalVirtualFileSetInfo : VirtualFileSetInfo
{
    public string Root { get; }

    public PhysicalVirtualFileSetInfo(
        [NotNull] IFileProvider fileProvider,
        [NotNull] string root
        )
        : base(fileProvider)
    {
        Root = Check.NotNullOrWhiteSpace(root, nameof(root));
    }
}
