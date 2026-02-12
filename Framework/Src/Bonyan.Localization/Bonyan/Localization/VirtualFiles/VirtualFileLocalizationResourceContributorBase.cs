using System.Linq;
using Bonyan.Exceptions;
using Bonyan.Internal;
using Bonyan.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;

namespace Bonyan.Localization.VirtualFiles;

public abstract class VirtualFileLocalizationResourceContributorBase : ILocalizationResourceContributor
{
    public bool IsDynamic => false;

    private readonly string _virtualPath;
    private IVirtualFileProvider _virtualFileProvider = default!;
    private Dictionary<string, ILocalizationDictionary>? _dictionaries;
    private bool _subscribedForChanges;
    private readonly object _syncObj = new object();
    private LocalizationResourceBase _resource = default!;

    protected VirtualFileLocalizationResourceContributorBase(string virtualPath)
    {
        _virtualPath = virtualPath;
    }

    public virtual void Initialize(LocalizationResourceInitializationContext context)
    {
        _resource = context.Resource;
        _virtualFileProvider = context.ServiceProvider.GetRequiredService<IVirtualFileProvider>();
    }

    public virtual LocalizedString? GetOrNull(string cultureName, string name)
    {
        var dictionaries = GetDictionaries();
        var dict = dictionaries.GetOrDefault(cultureName);
        var value = dict?.GetOrNull(name);
        // #region agent log
        try
        {
            var line = System.Text.Json.JsonSerializer.Serialize(new { hypothesisId = "H3_H5", location = "GetOrNull", message = "dict count and culture", data = new { resourceName = _resource?.ResourceName, virtualPath = _virtualPath, cultureName, name, dictCount = dictionaries.Count, dictKeys = dictionaries.Keys.ToList(), hasDictForCulture = dict != null, valueFound = value != null }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n";
            System.IO.File.AppendAllText(@"d:\Projects\Bonyan2\.cursor\debug.log", line);
        }
        catch { }
        // #endregion
        return value;
    }

    public virtual void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        GetDictionaries().GetOrDefault(cultureName)?.Fill(dictionary);
    }

    public Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        Fill(cultureName, dictionary);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
        return Task.FromResult((IEnumerable<string>)GetDictionaries().Keys);
    }

    private Dictionary<string, ILocalizationDictionary> GetDictionaries()
    {
        var dictionaries = _dictionaries;
        if (dictionaries != null)
        {
            return dictionaries;
        }

        lock (_syncObj)
        {
            dictionaries = _dictionaries;
            if (dictionaries != null)
            {
                return dictionaries;
            }

            if (!_subscribedForChanges)
            {
                ChangeToken.OnChange(() => _virtualFileProvider.Watch(_virtualPath.EnsureEndsWith('/') + "*.*"),
                    () =>
                    {
                        _dictionaries = null;
                    });

                _subscribedForChanges = true;
            }

            dictionaries = _dictionaries = CreateDictionaries();
        }

        return dictionaries;
    }

    private Dictionary<string, ILocalizationDictionary> CreateDictionaries()
    {
        var dictionaries = new Dictionary<string, ILocalizationDictionary>();
        // Try path without leading slash first (embedded provider stores paths without slash); then path with slash.
        var pathWithoutSlash = _virtualPath.TrimStart('/');
        var contents = pathWithoutSlash != _virtualPath
            ? _virtualFileProvider.GetDirectoryContents(pathWithoutSlash)
            : _virtualFileProvider.GetDirectoryContents(_virtualPath);
        var primaryCount = contents.Exists ? contents.Count(f => !f.IsDirectory && CanParseFile(f)) : 0;
        var usedAltPath = false;
        if (primaryCount == 0 && pathWithoutSlash != _virtualPath)
        {
            contents = _virtualFileProvider.GetDirectoryContents(_virtualPath);
            usedAltPath = true;
        }
        var altCount = contents.Exists ? contents.Count(f => !f.IsDirectory && CanParseFile(f)) : 0;
        // #region agent log
        try
        {
            var line = System.Text.Json.JsonSerializer.Serialize(new { hypothesisId = "H1_H2_H4", location = "CreateDictionaries", message = "virtual path and file counts", data = new { resourceName = _resource?.ResourceName, virtualPath = _virtualPath, triedPathWithoutSlashFirst = pathWithoutSlash != _virtualPath, contentsExists = contents.Exists, primaryCount, usedAltPath, altCount }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n";
            System.IO.File.AppendAllText(@"d:\Projects\Bonyan2\.cursor\debug.log", line);
        }
        catch { }
        // #endregion

        foreach (var file in contents)
        {
            if (file.IsDirectory || !CanParseFile(file))
            {
                continue;
            }

            var dictionary = CreateDictionaryFromFile(file);

            if (dictionary == null)
            {
                continue;
            }

            if (dictionaries.ContainsKey(dictionary.CultureName))
            {
                throw new BonException($"{file.GetVirtualOrPhysicalPathOrNull()} dictionary has a culture name '{dictionary.CultureName}' which is already defined! Localization resource: {_resource.ResourceName}");
            }

            dictionaries[dictionary.CultureName] = dictionary;
        }

        // #region agent log
        try
        {
            var line = System.Text.Json.JsonSerializer.Serialize(new { hypothesisId = "H1_H2_H4", location = "CreateDictionaries_result", message = "final dict count", data = new { resourceName = _resource?.ResourceName, virtualPath = _virtualPath, finalDictCount = dictionaries.Count, cultureKeys = dictionaries.Keys.ToList() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n";
            System.IO.File.AppendAllText(@"d:\Projects\Bonyan2\.cursor\debug.log", line);
        }
        catch { }
        // #endregion
        return dictionaries;
    }

    protected abstract bool CanParseFile(IFileInfo file);

    protected virtual ILocalizationDictionary? CreateDictionaryFromFile(IFileInfo file)
    {
        using (var stream = file.CreateReadStream())
        {
            return CreateDictionaryFromFileContent(Utf8Helper.ReadStringFromStream(stream));
        }
    }

    protected abstract ILocalizationDictionary? CreateDictionaryFromFileContent(string fileContent);
}
