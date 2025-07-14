using Bonyan.AspNetCore.Localization.Json;
using Microsoft.Extensions.FileProviders;

namespace Bonyan.AspNetCore.Localization.VirtualFiles.Json;

//TODO: Use composition over inheritance..?

public class JsonVirtualFileLocalizationResourceContributor : VirtualFileLocalizationResourceContributorBase
{
    public JsonVirtualFileLocalizationResourceContributor(string virtualPath)
        : base(virtualPath)
    {

    }

    protected override bool CanParseFile(IFileInfo file)
    {
        return file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
    }

    protected override ILocalizationDictionary? CreateDictionaryFromFileContent(string jsonString)
    {
        return JsonLocalizationDictionaryBuilder.BuildFromJsonString(jsonString);
    }
}
