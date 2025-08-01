﻿using System.Collections.Immutable;
using System.Globalization;
using System.Resources;
using Bonyan.Helpers;
using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public class BonDictionaryBasedStringLocalizer : IBonStringLocalizer
{
    public LocalizationResourceBase Resource { get; }

    public List<IStringLocalizer> BaseLocalizers { get; }

    public BonLocalizationOptions BonLocalizationOptions { get; }

    public virtual LocalizedString this[string name] => GetLocalizedString(name);

    public virtual LocalizedString this[string name, params object[] arguments] => GetLocalizedStringFormatted(name, arguments);

    public BonDictionaryBasedStringLocalizer(
        LocalizationResourceBase resource,
        List<IStringLocalizer> baseLocalizers,
        BonLocalizationOptions bonLocalizationOptions)
    {
        Resource = resource;
        BaseLocalizers = baseLocalizers;
        BonLocalizationOptions = bonLocalizationOptions;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return GetAllStrings(
            CultureInfo.CurrentUICulture.Name,
            includeParentCultures
        );
    }
    
    public async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(bool includeParentCultures)
    {
        return await GetAllStringsAsync(
            CultureInfo.CurrentUICulture.Name,
            includeParentCultures
        );
    }

    public IEnumerable<LocalizedString> GetAllStrings(
        bool includeParentCultures,
        bool includeBaseLocalizers,
        bool includeDynamicContributors)
    {
        return GetAllStrings(
            CultureInfo.CurrentUICulture.Name,
            includeParentCultures,
            includeBaseLocalizers,
            includeDynamicContributors
        );
    }

    public async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        bool includeParentCultures, 
        bool includeBaseLocalizers,
        bool includeDynamicContributors)
    {
        return await GetAllStringsAsync(
            CultureInfo.CurrentUICulture.Name,
            includeParentCultures,
            includeBaseLocalizers,
            includeDynamicContributors
        );
    }

    public Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
        return Resource.Contributors.GetSupportedCulturesAsync();
    }

    protected virtual LocalizedString GetLocalizedStringFormatted(string name, params object[] arguments)
    {
        return GetLocalizedStringFormatted(name, CultureInfo.CurrentUICulture.Name, arguments);
    }

    protected virtual LocalizedString GetLocalizedStringFormatted(string name, string cultureName, params object[] arguments)
    {
        var localizedString = GetLocalizedString(name, cultureName);
        return new LocalizedString(name, string.Format(localizedString.Value, arguments), localizedString.ResourceNotFound, localizedString.SearchedLocation);
    }

    protected virtual LocalizedString GetLocalizedString(string name)
    {
        return GetLocalizedString(name, CultureInfo.CurrentUICulture.Name);
    }

    protected virtual LocalizedString GetLocalizedString(string name, string cultureName)
    {
        var value = GetLocalizedStringOrNull(name, cultureName);

        if (value == null)
        {
            foreach (var baseLocalizer in BaseLocalizers)
            {
                using (CultureHelper.Use(CultureInfo.GetCultureInfo(cultureName)))
                {
                    var baseLocalizedString = baseLocalizer[name];
                    if (baseLocalizedString != null && !baseLocalizedString.ResourceNotFound)
                    {
                        return baseLocalizedString;
                    }
                }
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }

        return value;
    }

    protected virtual LocalizedString? GetLocalizedStringOrNull(
        string name,
        string cultureName,
        bool tryDefaults = true)
    {
        //Try to get from original dictionary (with country code)
        var strOriginal = Resource.Contributors.GetOrNull(cultureName, name);
        if (strOriginal != null)
        {
            return strOriginal;
        }

        if (!tryDefaults)
        {
            return null;
        }

        if (BonLocalizationOptions.TryToGetFromBaseCulture)
        {
            //Try to get from same language dictionary (without country code)
            if (cultureName.Contains("-")) //Example: "tr-TR" (length=5)
            {
                var strLang = Resource.Contributors.GetOrNull(CultureHelper.GetBaseCultureName(cultureName), name);
                if (strLang != null)
                {
                    return strLang;
                }
            }
        }

        if (BonLocalizationOptions.TryToGetFromDefaultCulture)
        {
            //Try to get from default language
            if (!Resource.DefaultCultureName.IsNullOrEmpty())
            {
                var strDefault = Resource.Contributors.GetOrNull(Resource.DefaultCultureName!, name);
                if (strDefault != null)
                {
                    return strDefault;
                }
            }
        }

        //Not found
        return null;
    }

    protected virtual IReadOnlyList<LocalizedString> GetAllStrings(
        string cultureName,
        bool includeParentCultures = true,
        bool includeBaseLocalizers = true,
        bool includeDynamicContributors = true)
    {
        //TODO: Can be optimized (example: if it's already default dictionary, skip overriding)

        var allStrings = new Dictionary<string, LocalizedString>();

        if (includeBaseLocalizers)
        {
            foreach (var baseLocalizer in BaseLocalizers.Select(l => l))
            {
                using (CultureHelper.Use(CultureInfo.GetCultureInfo(cultureName)))
                {
                    //TODO: Try/catch is a workaround here!
                    try
                    {
                        var baseLocalizedString = baseLocalizer.GetAllStrings(
                            includeParentCultures,
                            includeBaseLocalizers, // Always true, I know!
                            includeDynamicContributors
                        );
                        
                        foreach (var localizedString in baseLocalizedString)
                        {
                            allStrings[localizedString.Name] = localizedString;
                        }
                    }
                    catch (MissingManifestResourceException)
                    {

                    }
                }
            }
        }

        if (includeParentCultures)
        {
            //Fill all strings from default culture
            if (!Resource.DefaultCultureName.IsNullOrEmpty())
            {
                Resource.Contributors.Fill(Resource.DefaultCultureName!, allStrings, includeDynamicContributors);
            }

            //Overwrite all strings from the language based on country culture
            if (cultureName.Contains("-"))
            {
                Resource.Contributors.Fill(CultureHelper.GetBaseCultureName(cultureName), allStrings, includeDynamicContributors);
            }
        }

        //Overwrite all strings from the original culture
        Resource.Contributors.Fill(cultureName, allStrings, includeDynamicContributors);

        return allStrings.Values.ToImmutableList();
    }

    protected virtual async Task<IReadOnlyList<LocalizedString>> GetAllStringsAsync(
        string cultureName,
        bool includeParentCultures = true,
        bool includeBaseLocalizers = true,
        bool includeDynamicContributors = true)
    {
        //TODO: Can be optimized (example: if it's already default dictionary, skip overriding)

        var allStrings = new Dictionary<string, LocalizedString>();

        if (includeBaseLocalizers)
        {
            foreach (var baseLocalizer in BaseLocalizers.Select(l => l))
            {
                using (CultureHelper.Use(CultureInfo.GetCultureInfo(cultureName)))
                {
                    //TODO: Try/catch is a workaround here!
                    try
                    {
                        var baseLocalizedString = await baseLocalizer.GetAllStringsAsync(
                            includeParentCultures,
                            includeBaseLocalizers, // Always true, I know!
                            includeDynamicContributors
                        );
                        
                        foreach (var localizedString in baseLocalizedString)
                        {
                            allStrings[localizedString.Name] = localizedString;
                        }
                    }
                    catch (MissingManifestResourceException)
                    {

                    }
                }
            }
        }

        if (includeParentCultures)
        {
            //Fill all strings from default culture
            if (!Resource.DefaultCultureName.IsNullOrEmpty())
            {
                await Resource.Contributors.FillAsync(
                    Resource.DefaultCultureName!,
                    allStrings,
                    includeDynamicContributors
                );
            }

            //Overwrite all strings from the language based on country culture
            if (cultureName.Contains("-"))
            {
                await Resource.Contributors.FillAsync(
                    CultureHelper.GetBaseCultureName(cultureName),
                    allStrings,
                    includeDynamicContributors
                );
            }
        }

        //Overwrite all strings from the original culture
        await Resource.Contributors.FillAsync(
            cultureName,
            allStrings,
            includeDynamicContributors
        );

        return allStrings.Values.ToImmutableList();
    }

    public class CultureWrapperStringLocalizer : IBonStringLocalizer
    {
        private readonly string _cultureName;
        private readonly BonDictionaryBasedStringLocalizer _innerLocalizer;

        LocalizedString IStringLocalizer.this[string name] => _innerLocalizer.GetLocalizedString(name, _cultureName);

        LocalizedString IStringLocalizer.this[string name, params object[] arguments] => _innerLocalizer.GetLocalizedStringFormatted(name, _cultureName, arguments);

        public CultureWrapperStringLocalizer(string cultureName, BonDictionaryBasedStringLocalizer innerLocalizer)
        {
            _cultureName = cultureName;
            _innerLocalizer = innerLocalizer;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _innerLocalizer.GetAllStrings(_cultureName, includeParentCultures);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, bool includeBaseLocalizers, bool includeDynamicContributors)
        {
            return _innerLocalizer.GetAllStrings(_cultureName, includeParentCultures, includeBaseLocalizers, includeDynamicContributors);
        }

        public Task<IEnumerable<LocalizedString>> GetAllStringsAsync(bool includeParentCultures)
        {
            return _innerLocalizer.GetAllStringsAsync(includeParentCultures);
        }

        public Task<IEnumerable<LocalizedString>> GetAllStringsAsync(bool includeParentCultures, bool includeBaseLocalizers, bool includeDynamicContributors)
        {
            return _innerLocalizer.GetAllStringsAsync(
                includeParentCultures,
                includeBaseLocalizers,
                includeDynamicContributors
            );
        }

        public Task<IEnumerable<string>> GetSupportedCulturesAsync()
        {
            return _innerLocalizer.GetSupportedCulturesAsync();
        }
    }
}
