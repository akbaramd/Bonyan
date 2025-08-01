﻿using System.Reflection;
using Bonyan.Core;
using Bonyan.VirtualFileSystem.Embedded;
using Bonyan.VirtualFileSystem.Physical;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Bonyan.VirtualFileSystem;

public static class VirtualFileSetListExtensions
{
    public static void AddEmbedded<T>(
        [NotNull] this VirtualFileSetList list,
        string? baseNamespace = null,
        string? baseFolder = null)
    {
        Check.NotNull(list, nameof(list));

        var assembly = typeof(T).Assembly;
        var fileProvider = CreateFileProvider(
            assembly,
            baseNamespace,
            baseFolder
        );

        list.Add(new EmbeddedVirtualFileSetInfo(fileProvider, assembly, baseFolder));
    }

    public static void AddPhysical(
        [NotNull] this VirtualFileSetList list,
        [NotNull] string root,
        ExclusionFilters exclusionFilters = ExclusionFilters.Sensitive)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNullOrWhiteSpace(root, nameof(root));

        var fileProvider = new PhysicalFileProvider(root, exclusionFilters);
        list.Add(new PhysicalVirtualFileSetInfo(fileProvider, root));
    }

    private static IFileProvider CreateFileProvider(
        [NotNull] Assembly assembly,
        string? baseNamespace = null,
        string? baseFolder = null)
    {
        Check.NotNull(assembly, nameof(assembly));

        var info = assembly.GetManifestResourceInfo("Microsoft.Extensions.FileProviders.Embedded.Manifest.xml");

        if (info == null)
        {
            return new BonEmbeddedFileProvider(assembly, baseNamespace);
        }

        if (baseFolder == null)
        {
            return new ManifestEmbeddedFileProvider(assembly);
        }

        return new ManifestEmbeddedFileProvider(assembly, baseFolder);
    }

    public static void ReplaceEmbeddedByPhysical<T>(
        [NotNull] this VirtualFileSetList fileSets,
        [NotNull] string physicalPath)
    {
        Check.NotNull(fileSets, nameof(fileSets));
        Check.NotNullOrWhiteSpace(physicalPath, nameof(physicalPath));

        var assembly = typeof(T).Assembly;

        for (var i = 0; i < fileSets.Count; i++)
        {
            if (fileSets[i] is EmbeddedVirtualFileSetInfo embeddedVirtualFileSet &&
                embeddedVirtualFileSet.Assembly == assembly)
            {
                var thisPath = physicalPath;

                if (!embeddedVirtualFileSet.BaseFolder.IsNullOrEmpty())
                {
                    thisPath = Path.Combine(thisPath, embeddedVirtualFileSet.BaseFolder!);
                }

                fileSets[i] = new PhysicalVirtualFileSetInfo(new PhysicalFileProvider(thisPath), thisPath);
            }
        }
    }
}
