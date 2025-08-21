using Bonyan.AspNetCore.ZoneComponent;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Extension methods for rendering zones with HtmlHelper
/// </summary>
public static class HtmlHelperZoneExtensions
{


    /// <summary>
    /// Render a zone with optional payload
    /// </summary>
    public static async Task<IHtmlContent> RenderZoneAsync(this IHtmlHelper html, string zone, object? payload = null)
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var components = registry.Get(zone);

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);

        foreach (var component in components)
        {
            try
            {
                var result = await component.HandleAsync(payload, context);
                
                if (result.HtmlContent != null)
                {
                    builder.AppendHtml(result.HtmlContent);
                }
                else if (!string.IsNullOrEmpty(result.ViewName))
                {
                    var viewHtml = await context.RenderPartialViewAsHtmlAsync(result.ViewName, result.ViewModel);
                    builder.AppendHtml(viewHtml);
                }

                // Stop processing if requested
                if (!result.ContinueProcessing)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        return builder;
    }

    /// <summary>
    /// Render a tab zone with proper tab structure
    /// </summary>
    public static async Task<IHtmlContent> RenderTabZoneAsync(this IHtmlHelper html, string zone, object? payload = null, string? containerId = null, string? containerClasses = null)
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var components = registry.Get(zone);

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);

        var tabComponents = new List<(IZoneComponent Component, TabContentResult? TabResult)>();
        var regularComponents = new List<(IZoneComponent Component, ZoneComponentResult Result)>();

        // Process all components and separate tab components from regular components
        foreach (var component in components)
        {
            try
            {
                var result = await component.HandleAsync(payload, context);
                
                // Check if it's a tab component result
                if (result is TabContentResult tabResult)
                {
                    if (tabResult.IsTabContent)
                    {
                        tabComponents.Add((component, tabResult));
                    }
                    else
                    {
                        regularComponents.Add((component, tabResult));
                    }
                }
                else
                {
                    regularComponents.Add((component, result));
                }
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        // Render tab structure if we have tab components
        if (tabComponents.Any())
        {
            var tabContainerId = containerId ?? $"tab-container-{zone.Replace(" ", "-").ToLowerInvariant()}";
            var tabContainerClasses = containerClasses ?? "tab-container";

            // Generate tab headers
            var tabHeaders = new HtmlContentBuilder();
            var tabContents = new HtmlContentBuilder();

            // Sort tab components by order
            var orderedTabComponents = tabComponents
                .Where(tc => tc.TabResult != null)
                .OrderBy(tc => tc.TabResult!.TabParameters.Order)
                .ToList();

            // Ensure only one tab is active at a time
            var activeTabFound = false;
            foreach (var (component, tabResult) in orderedTabComponents)
            {
                if (tabResult?.TabParameters.IsActive == true)
                {
                    if (activeTabFound)
                    {
                        // If we already found an active tab, make this one inactive
                        tabResult.TabParameters.IsActive = false;
                    }
                    else
                    {
                        activeTabFound = true;
                    }
                }
            }

            // If no active tab found, make the first tab active
            if (!activeTabFound && orderedTabComponents.Any())
            {
                var firstTab = orderedTabComponents.First();
                if (firstTab.TabResult != null)
                {
                    firstTab.TabResult.TabParameters.IsActive = true;
                }
            }

            foreach (var (component, tabResult) in orderedTabComponents)
            {
                if (tabResult == null) continue;

                // Add tab header
                tabHeaders.AppendHtml(GenerateTabHeader(tabResult.TabParameters));

                // Add tab content with proper Bootstrap classes
                var activeClass = tabResult.TabParameters.IsActive ? "show active" : "";
                var tabPaneClasses = $"tab-pane fade {activeClass}".Trim();
                
                tabContents.AppendHtml($"<div class=\"{tabPaneClasses}\" id=\"{tabResult.TabParameters.TabId}\" role=\"tabpanel\" aria-labelledby=\"{tabResult.TabParameters.TabId}-tab\">");
                
                if (tabResult.HtmlContent != null)
                {
                    tabContents.AppendHtml(tabResult.HtmlContent);
                }
                else if (!string.IsNullOrEmpty(tabResult.ViewName))
                {
                    var viewHtml = await context.RenderPartialViewAsHtmlAsync(tabResult.ViewName, tabResult.ViewModel);
                    tabContents.AppendHtml(viewHtml);
                }
                
                tabContents.AppendHtml("</div>");

                // Stop processing if requested
                if (!tabResult.ContinueProcessing)
                    break;
            }

            // Build complete tab structure
            builder.AppendHtml($"<div id=\"{tabContainerId}\" class=\"{tabContainerClasses}\">");
            builder.AppendHtml("<ul class=\"nav nav-tabs\" role=\"tablist\">");
            builder.AppendHtml(tabHeaders);
            builder.AppendHtml("</ul>");
            builder.AppendHtml("<div class=\"tab-content\">");
            builder.AppendHtml(tabContents);
            builder.AppendHtml("</div>");
            builder.AppendHtml("</div>");
        }

        // Render regular components
        foreach (var (component, result) in regularComponents)
        {
            if (result.HtmlContent != null)
            {
                builder.AppendHtml(result.HtmlContent);
            }
            else if (!string.IsNullOrEmpty(result.ViewName))
            {
                var viewHtml = await context.RenderPartialViewAsHtmlAsync(result.ViewName, result.ViewModel);
                builder.AppendHtml(viewHtml);
            }

            // Stop processing if requested
            if (!result.ContinueProcessing)
                break;
        }

        return builder;
    }

    /// <summary>
    /// Render a tab zone with strongly typed payload
    /// </summary>
    public static async Task<IHtmlContent> RenderTabZoneAsync<T>(this IHtmlHelper html, string zone, T payload, string? containerId = null, string? containerClasses = null)
    {
        return await RenderTabZoneAsync(html, zone, (object?)payload, containerId, containerClasses);
    }

    /// <summary>
    /// Render a tab zone using a generic model that implements IZone
    /// </summary>
    public static async Task<IHtmlContent> RenderTabZoneAsync<TZone>(this IHtmlHelper html, TZone model, string? containerId = null, string? containerClasses = null) where TZone : IZone
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        return await RenderTabZoneAsync(html, model.ZoneName, model, containerId, containerClasses);
    }

    /// <summary>
    /// Render only tab headers for a zone
    /// </summary>
    public static async Task<IHtmlContent> RenderTabHeadersAsync(this IHtmlHelper html, string zone, object? payload = null, string? containerClasses = null)
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var components = registry.Get(zone);

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);

        var tabHeaders = new HtmlContentBuilder();

        foreach (var component in components)
        {
            try
            {
                var result = await component.HandleAsync(payload, context);
                
                if (result is TabContentResult tabResult && tabResult.IsTabContent)
                {
                    tabHeaders.AppendHtml(GenerateTabHeader(tabResult.TabParameters));
                }

                // Stop processing if requested
                if (!result.ContinueProcessing)
                    break;
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        var classes = containerClasses ?? "nav nav-tabs";
        builder.AppendHtml($"<ul class=\"{classes}\" role=\"tablist\">");
        builder.AppendHtml(tabHeaders);
        builder.AppendHtml("</ul>");

        return builder;
    }

    /// <summary>
    /// Render only tab contents for a zone
    /// </summary>
    public static async Task<IHtmlContent> RenderTabContentsAsync(this IHtmlHelper html, string zone, object? payload = null, string? containerClasses = null)
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var components = registry.Get(zone);

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);

        var tabComponents = new List<TabContentResult>();
        var tabContents = new HtmlContentBuilder();

        // First pass: collect all tab components
        foreach (var component in components)
        {
            try
            {
                var result = await component.HandleAsync(payload, context);
                
                if (result is TabContentResult tabResult && tabResult.IsTabContent)
                {
                    tabComponents.Add(tabResult);
                }

                // Stop processing if requested
                if (!result.ContinueProcessing)
                    break;
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        // Sort tab components by order
        var orderedTabComponents = tabComponents
            .OrderBy(tc => tc.TabParameters.Order)
            .ToList();

        // Ensure only one tab is active at a time
        var activeTabFound = false;
        foreach (var tabResult in orderedTabComponents)
        {
            if (tabResult.TabParameters.IsActive)
            {
                if (activeTabFound)
                {
                    // If we already found an active tab, make this one inactive
                    tabResult.TabParameters.IsActive = false;
                }
                else
                {
                    activeTabFound = true;
                }
            }
        }

        // If no active tab found, make the first tab active
        if (!activeTabFound && orderedTabComponents.Any())
        {
            var firstTab = orderedTabComponents.First();
            firstTab.TabParameters.IsActive = true;
        }

        // Second pass: render tab contents
        foreach (var tabResult in orderedTabComponents)
        {
            // Add tab content with proper Bootstrap classes
            var activeClass = tabResult.TabParameters.IsActive ? "show active" : "";
            var tabPaneClasses = $"tab-pane fade {activeClass}".Trim();
            
            tabContents.AppendHtml($"<div class=\"{tabPaneClasses}\" id=\"{tabResult.TabParameters.TabId}\" role=\"tabpanel\" aria-labelledby=\"{tabResult.TabParameters.TabId}-tab\">");
            
            if (tabResult.HtmlContent != null)
            {
                tabContents.AppendHtml(tabResult.HtmlContent);
            }
            else if (!string.IsNullOrEmpty(tabResult.ViewName))
            {
                var viewHtml = await context.RenderPartialViewAsHtmlAsync(tabResult.ViewName, tabResult.ViewModel);
                tabContents.AppendHtml(viewHtml);
            }
            
            tabContents.AppendHtml("</div>");
        }

        var classes = containerClasses ?? "tab-content";
        builder.AppendHtml($"<div class=\"{classes}\">");
        builder.AppendHtml(tabContents);
        builder.AppendHtml("</div>");

        return builder;
    }

    /// <summary>
    /// Generate tab header HTML
    /// </summary>
    private static IHtmlContent GenerateTabHeader(TabParameters tabParameters)
    {
        var activeClass = tabParameters.IsActive ? "active" : "";
        var disabledClass = tabParameters.IsDisabled ? "disabled" : "";
        var classes = $"nav-link {activeClass} {disabledClass} {tabParameters.CssClasses}".Trim();

        var attributes = string.Join(" ", tabParameters.Attributes.Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""));
        if (!string.IsNullOrEmpty(attributes))
            attributes = " " + attributes;

        var tooltipAttr = !string.IsNullOrEmpty(tabParameters.Tooltip) ? $" title=\"{tabParameters.Tooltip}\"" : "";
        var dataAttributes = tabParameters.LazyLoad ? " data-bs-toggle=\"tab\" data-lazy-load=\"true\"" : " data-bs-toggle=\"tab\"";

        var iconHtml = !string.IsNullOrEmpty(tabParameters.TabIcon) ? $"<i class=\"{tabParameters.TabIcon}\"></i> " : "";
        var badgeHtml = !string.IsNullOrEmpty(tabParameters.BadgeText) ? $" <span class=\"{tabParameters.BadgeClasses}\">{tabParameters.BadgeText}</span>" : "";

        var headerHtml = $"<li class=\"nav-item\" role=\"presentation\">" +
                        $"<button class=\"{classes}\" id=\"{tabParameters.TabId}-tab\" data-bs-target=\"#{tabParameters.TabId}\" type=\"button\" role=\"tab\"{tooltipAttr}{dataAttributes}{attributes}>" +
                        $"{iconHtml}{tabParameters.TabText}{badgeHtml}" +
                        $"</button>" +
                        $"</li>";

        return new HtmlString(headerHtml);
    }

    /// <summary>
    /// Render a zone with strongly typed payload
    /// </summary>
    public static async Task<IHtmlContent> RenderZoneAsync<T>(this IHtmlHelper html, string zone, T payload)
    {
        return await RenderZoneAsync(html, zone, (object?)payload);
    }

    /// <summary>
    /// Render a zone with generic attributes
    /// </summary>
    public static async Task<IHtmlContent> RenderZoneAsync<T, TComponent>(this IHtmlHelper html, string zone, T payload) 
        where TComponent : class, IZoneComponent
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var components = registry.Get(zone).OfType<TComponent>();

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);


        foreach (var component in components)
        {
            // Skip if already rendered in this request
            var componentKey = $"{zone}_{component.Id}";
            

            try
            {
                var result = await component.HandleAsync(payload, context);
                
                if (result.HtmlContent != null)
                {
                    builder.AppendHtml(result.HtmlContent);
                }
                else if (!string.IsNullOrEmpty(result.ViewName))
                {
                    var viewHtml = await context.RenderPartialViewAsHtmlAsync(result.ViewName, result.ViewModel);
                    builder.AppendHtml(viewHtml);
                }

                // Stop processing if requested
                if (!result.ContinueProcessing)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        return builder;
    }

    /// <summary>
    /// Render a zone with multiple generic attributes
    /// </summary>
    public static async Task<IHtmlContent> RenderZoneAsync<T, TComponent1, TComponent2>(this IHtmlHelper html, string zone, T payload) 
        where TComponent1 : class, IZoneComponent
        where TComponent2 : class, IZoneComponent
    {
        var registry = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
        var allComponents = registry.Get(zone);
        var typedComponents = allComponents.OfType<TComponent1>().Cast<IZoneComponent>()
            .Concat(allComponents.OfType<TComponent2>().Cast<IZoneComponent>())
            .OrderBy(c => c.Priority);

        var builder = new HtmlContentBuilder();
        using var context = new ZoneRenderingContext(
            html.ViewContext, 
            html.ViewContext.HttpContext.RequestServices, 
            zone, 
            payload);

        foreach (var component in typedComponents)
        {
            try
            {
                var result = await component.HandleAsync(payload, context);
                
                if (result.HtmlContent != null)
                {
                    builder.AppendHtml(result.HtmlContent);
                }
                else if (!string.IsNullOrEmpty(result.ViewName))
                {
                    var viewHtml = await context.RenderPartialViewAsHtmlAsync(result.ViewName, result.ViewModel);
                    builder.AppendHtml(viewHtml);
                }

                // Stop processing if requested
                if (!result.ContinueProcessing)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                // Log error and continue with other components
                builder.AppendHtml($"<!-- Error rendering component {component.GetType().Name}: {ex.Message} -->");
            }
        }

        return builder;
    }

    /// <summary>
    /// Render a zone using a generic model that implements IZone (zone name is taken from the model)
    /// </summary>
    public static async Task<IHtmlContent> RenderZoneAsync<TZone>(this IHtmlHelper html, TZone model) where TZone : IZone
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        return await RenderZoneAsync(html, model.ZoneName, model);
    }


} 