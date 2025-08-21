using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Tab-specific parameters for zone tab components
/// </summary>
public class TabParameters
{
    /// <summary>
    /// Unique identifier for the tab
    /// </summary>
    public string TabId { get; set; } = string.Empty;

    /// <summary>
    /// Icon class or path for the tab
    /// </summary>
    public string TabIcon { get; set; } = string.Empty;

    /// <summary>
    /// Display text for the tab
    /// </summary>
    public string TabText { get; set; } = string.Empty;

    /// <summary>
    /// Whether the tab is active by default
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the tab is disabled
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// CSS classes for the tab
    /// </summary>
    public string CssClasses { get; set; } = string.Empty;

    /// <summary>
    /// Additional attributes for the tab
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();

    /// <summary>
    /// Tab order/priority
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Whether the tab content should be loaded lazily
    /// </summary>
    public bool LazyLoad { get; set; }

    /// <summary>
    /// Tooltip text for the tab
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Badge text to display on the tab
    /// </summary>
    public string? BadgeText { get; set; }

    /// <summary>
    /// Badge CSS classes
    /// </summary>
    public string BadgeClasses { get; set; } = "badge bg-primary";

    public TabParameters()
    {
    }

    public TabParameters(string tabId, string tabText, string tabIcon = "", bool isActive = false)
    {
        TabId = tabId;
        TabText = tabText;
        TabIcon = tabIcon;
        IsActive = isActive;
    }

    /// <summary>
    /// Add a custom attribute to the tab
    /// </summary>
    public TabParameters AddAttribute(string key, string value)
    {
        Attributes[key] = value;
        return this;
    }

    /// <summary>
    /// Set the tab as active
    /// </summary>
    public TabParameters SetActive()
    {
        IsActive = true;
        return this;
    }

    /// <summary>
    /// Set the tab as disabled
    /// </summary>
    public TabParameters SetDisabled()
    {
        IsDisabled = true;
        return this;
    }

    /// <summary>
    /// Enable lazy loading for the tab content
    /// </summary>
    public TabParameters SetLazyLoad()
    {
        LazyLoad = true;
        return this;
    }

    /// <summary>
    /// Add CSS classes to the tab
    /// </summary>
    public TabParameters AddCssClasses(string classes)
    {
        if (!string.IsNullOrEmpty(CssClasses))
            CssClasses += " ";
        CssClasses += classes;
        return this;
    }
}

/// <summary>
/// Tab content result with enhanced tab-specific properties
/// </summary>
public class TabContentResult : ZoneComponentResult
{
    /// <summary>
    /// Tab parameters for this content
    /// </summary>
    public TabParameters TabParameters { get; set; }

    /// <summary>
    /// Whether this tab should be rendered as a separate tab
    /// </summary>
    public bool IsTabContent { get; set; } = true;

    /// <summary>
    /// Create a tab content result with HTML
    /// </summary>
    public static new TabContentResult Html(IHtmlContent htmlContent, TabParameters tabParameters, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new TabContentResult
        {
            HtmlContent = htmlContent,
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta,
            TabParameters = tabParameters,
            IsTabContent = true
        };
    }

    /// <summary>
    /// Create a tab content result with HTML string
    /// </summary>
    public static new TabContentResult Html(string html, TabParameters tabParameters, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new TabContentResult
        {
            HtmlContent = new HtmlString(html),
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta,
            TabParameters = tabParameters,
            IsTabContent = true
        };
    }

    /// <summary>
    /// Create a tab content result with a view
    /// </summary>
    public static new TabContentResult View(string viewName, TabParameters tabParameters, object? viewModel = null, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new TabContentResult
        {
            ViewName = viewName,
            ViewModel = viewModel,
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta,
            TabParameters = tabParameters,
            IsTabContent = true
        };
    }

    /// <summary>
    /// Create a non-tab content result (regular zone content)
    /// </summary>
    public static TabContentResult RegularContent(IHtmlContent htmlContent, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new TabContentResult
        {
            HtmlContent = htmlContent,
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta,
            TabParameters = new TabParameters(),
            IsTabContent = false
        };
    }

    /// <summary>
    /// Create a non-tab content result with HTML string
    /// </summary>
    public static TabContentResult RegularContent(string html, bool continueProcessing = true, ZoneComponentParameters? parameters = null, IDictionary<string, object>? meta = null)
    {
        return new TabContentResult
        {
            HtmlContent = new HtmlString(html),
            ContinueProcessing = continueProcessing,
            Parameters = parameters,
            Meta = meta,
            TabParameters = new TabParameters(),
            IsTabContent = false
        };
    }
}

/// <summary>
/// Advanced zone tab view component with tab-specific parameters and rendering capabilities
/// </summary>
/// <typeparam name="TModel">The type of the input model that must implement IZone</typeparam>
public abstract class ZoneTabViewComponent<TModel> : ZoneViewComponent<TModel> where TModel : IZone
{
    /// <summary>
    /// Tab parameters for this component
    /// </summary>
    public TabParameters TabParameters { get; protected set; }

    /// <summary>
    /// Whether this component should render as a tab
    /// </summary>
    public virtual bool IsTabComponent => true;

    /// <summary>
    /// Tab container ID for grouping related tabs
    /// </summary>
    public virtual string TabContainerId => $"tab-container-{Id}";

    /// <summary>
    /// Default tab parameters
    /// </summary>
    protected virtual TabParameters DefaultTabParameters => new TabParameters();

    protected ZoneTabViewComponent()
    {
        TabParameters = DefaultTabParameters;
    }

    /// <summary>
    /// Initialize tab parameters with default values
    /// </summary>
    protected override void InitializeParameters()
    {
        base.InitializeParameters();

        // Add tab-specific parameters
        Parameters.AddParameter("tabId", false, TabParameters.TabId, "Unique identifier for the tab");
        Parameters.AddParameter("tabText", false, TabParameters.TabText, "Display text for the tab");
        Parameters.AddParameter("tabIcon", false, TabParameters.TabIcon, "Icon class or path for the tab");
        Parameters.AddParameter("isActive", false, TabParameters.IsActive, "Whether the tab is active by default");
        Parameters.AddParameter("isDisabled", false, TabParameters.IsDisabled, "Whether the tab is disabled");
        Parameters.AddParameter("cssClasses", false, TabParameters.CssClasses, "CSS classes for the tab");
        Parameters.AddParameter("order", false, TabParameters.Order, "Tab order/priority");
        Parameters.AddParameter("lazyLoad", false, TabParameters.LazyLoad, "Whether the tab content should be loaded lazily");
        Parameters.AddParameter("tooltip", false, TabParameters.Tooltip, "Tooltip text for the tab");
        Parameters.AddParameter("badgeText", false, TabParameters.BadgeText, "Badge text to display on the tab");
        Parameters.AddParameter("badgeClasses", false, TabParameters.BadgeClasses, "Badge CSS classes");
    }

    /// <summary>
    /// Update tab parameters from component parameters
    /// </summary>
    protected virtual void UpdateTabParameters()
    {
        TabParameters.TabId = Parameters.GetValue<string>("tabId") ?? TabParameters.TabId;
        TabParameters.TabText = Parameters.GetValue<string>("tabText") ?? TabParameters.TabText;
        TabParameters.TabIcon = Parameters.GetValue<string>("tabIcon") ?? TabParameters.TabIcon;
        TabParameters.IsActive = Parameters.GetValue<bool>("isActive");
        TabParameters.IsDisabled = Parameters.GetValue<bool>("isDisabled");
        TabParameters.CssClasses = Parameters.GetValue<string>("cssClasses") ?? TabParameters.CssClasses;
        TabParameters.Order = Parameters.GetValue<int>("order");
        TabParameters.LazyLoad = Parameters.GetValue<bool>("lazyLoad");
        TabParameters.Tooltip = Parameters.GetValue<string>("tooltip");
        TabParameters.BadgeText = Parameters.GetValue<string>("badgeText");
        TabParameters.BadgeClasses = Parameters.GetValue<string>("badgeClasses") ?? TabParameters.BadgeClasses;
    }

    /// <summary>
    /// Handle the zone component with tab-specific logic
    /// </summary>
    public override async Task<ZoneComponentResult> HandleAsync(TModel model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        // Update tab parameters from component parameters
        UpdateTabParameters();

        // Call the tab-specific handler
        var result = await HandleTabAsync(model, context, parameters, cancellationToken);

        // If the result is not already a TabContentResult, wrap it
        if (result is not TabContentResult tabResult)
        {
            return TabContentResult.Html(result.HtmlContent ?? new HtmlString(""), TabParameters, result.ContinueProcessing, result.Parameters, result.Meta);
        }

        return tabResult;
    }

    /// <summary>
    /// Handle the tab component with tab-specific logic
    /// Override this method to implement tab-specific behavior
    /// </summary>
    /// <param name="model">The strongly typed input model</param>
    /// <param name="context">The zone rendering context</param>
    /// <param name="parameters">The component parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tab content result</returns>
    public abstract Task<TabContentResult> HandleTabAsync(TModel model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Render tab header HTML
    /// </summary>
    protected virtual string RenderTabHeader()
    {
        var activeClass = TabParameters.IsActive ? "active" : "";
        var disabledClass = TabParameters.IsDisabled ? "disabled" : "";
        var classes = $"nav-link {activeClass} {disabledClass} {TabParameters.CssClasses}".Trim();

        var attributes = string.Join(" ", TabParameters.Attributes.Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""));
        if (!string.IsNullOrEmpty(attributes))
            attributes = " " + attributes;

        var tooltipAttr = !string.IsNullOrEmpty(TabParameters.Tooltip) ? $" title=\"{TabParameters.Tooltip}\"" : "";
        var dataAttributes = TabParameters.LazyLoad ? " data-bs-toggle=\"tab\" data-lazy-load=\"true\"" : " data-bs-toggle=\"tab\"";

        var iconHtml = !string.IsNullOrEmpty(TabParameters.TabIcon) ? $"<i class=\"{TabParameters.TabIcon}\"></i> " : "";
        var badgeHtml = !string.IsNullOrEmpty(TabParameters.BadgeText) ? $" <span class=\"{TabParameters.BadgeClasses}\">{TabParameters.BadgeText}</span>" : "";

        return $"<li class=\"nav-item\" role=\"presentation\">" +
               $"<button class=\"{classes}\" id=\"{TabParameters.TabId}-tab\" data-bs-target=\"#{TabParameters.TabId}\" type=\"button\" role=\"tab\"{tooltipAttr}{dataAttributes}{attributes}>" +
               $"{iconHtml}{TabParameters.TabText}{badgeHtml}" +
               $"</button>" +
               $"</li>";
    }

    /// <summary>
    /// Render tab content HTML
    /// </summary>
    protected virtual string RenderTabContent(string content)
    {
        var activeClass = TabParameters.IsActive ? "show active" : "";
        var lazyLoadAttr = TabParameters.LazyLoad ? " data-lazy-loaded=\"false\"" : "";

        return $"<div class=\"tab-pane fade {activeClass}\" id=\"{TabParameters.TabId}\" role=\"tabpanel\" aria-labelledby=\"{TabParameters.TabId}-tab\"{lazyLoadAttr}>" +
               $"<div class=\"tab-content-wrapper\">" +
               $"{content}" +
               $"</div>" +
               $"</div>";
    }

    /// <summary>
    /// Render complete tab structure
    /// </summary>
    protected virtual string RenderCompleteTab(string content)
    {
        return RenderTabHeader() + "\n" + RenderTabContent(content);
    }

    /// <summary>
    /// Create a tab content result with rendered tab structure
    /// </summary>
    protected TabContentResult CreateTabResult(string content, bool continueProcessing = true, ZoneComponentParameters? parameters = null)
    {
        var htmlContent = new HtmlString(content);
        return TabContentResult.Html(htmlContent, TabParameters, continueProcessing, parameters);
    }

    /// <summary>
    /// Create a regular content result (not as a tab)
    /// </summary>
    protected TabContentResult CreateRegularContentResult(string content, bool continueProcessing = true, ZoneComponentParameters? parameters = null)
    {
        var htmlContent = new HtmlString(content);
        return TabContentResult.RegularContent(htmlContent, continueProcessing, parameters);
    }
}

/// <summary>
/// Advanced zone tab view component with context and tab-specific parameters
/// </summary>
/// <typeparam name="TModel">The type of the input model</typeparam>
/// <typeparam name="TContext">The type of the additional context</typeparam>
public abstract class ZoneTabViewComponent<TModel, TContext> : ZoneViewComponent<TModel, TContext>
{
    /// <summary>
    /// Tab parameters for this component
    /// </summary>
    public TabParameters TabParameters { get; protected set; }

    /// <summary>
    /// Whether this component should render as a tab
    /// </summary>
    public virtual bool IsTabComponent => true;

    /// <summary>
    /// Tab container ID for grouping related tabs
    /// </summary>
    public virtual string TabContainerId => $"tab-container-{Id}";

    /// <summary>
    /// Default tab parameters
    /// </summary>
    protected virtual TabParameters DefaultTabParameters => new TabParameters();

    protected ZoneTabViewComponent()
    {
        TabParameters = DefaultTabParameters;
    }

    /// <summary>
    /// Initialize tab parameters with default values
    /// </summary>
    protected override void InitializeParameters()
    {
        base.InitializeParameters();

        // Add tab-specific parameters
        Parameters.AddParameter("tabId", false, TabParameters.TabId, "Unique identifier for the tab");
        Parameters.AddParameter("tabText", false, TabParameters.TabText, "Display text for the tab");
        Parameters.AddParameter("tabIcon", false, TabParameters.TabIcon, "Icon class or path for the tab");
        Parameters.AddParameter("isActive", false, TabParameters.IsActive, "Whether the tab is active by default");
        Parameters.AddParameter("isDisabled", false, TabParameters.IsDisabled, "Whether the tab is disabled");
        Parameters.AddParameter("cssClasses", false, TabParameters.CssClasses, "CSS classes for the tab");
        Parameters.AddParameter("order", false, TabParameters.Order, "Tab order/priority");
        Parameters.AddParameter("lazyLoad", false, TabParameters.LazyLoad, "Whether the tab content should be loaded lazily");
        Parameters.AddParameter("tooltip", false, TabParameters.Tooltip, "Tooltip text for the tab");
        Parameters.AddParameter("badgeText", false, TabParameters.BadgeText, "Badge text to display on the tab");
        Parameters.AddParameter("badgeClasses", false, TabParameters.BadgeClasses, "Badge CSS classes");
    }

    /// <summary>
    /// Update tab parameters from component parameters
    /// </summary>
    protected virtual void UpdateTabParameters()
    {
        TabParameters.TabId = Parameters.GetValue<string>("tabId") ?? TabParameters.TabId;
        TabParameters.TabText = Parameters.GetValue<string>("tabText") ?? TabParameters.TabText;
        TabParameters.TabIcon = Parameters.GetValue<string>("tabIcon") ?? TabParameters.TabIcon;
        TabParameters.IsActive = Parameters.GetValue<bool>("isActive");
        TabParameters.IsDisabled = Parameters.GetValue<bool>("isDisabled");
        TabParameters.CssClasses = Parameters.GetValue<string>("cssClasses") ?? TabParameters.CssClasses;
        TabParameters.Order = Parameters.GetValue<int>("order");
        TabParameters.LazyLoad = Parameters.GetValue<bool>("lazyLoad");
        TabParameters.Tooltip = Parameters.GetValue<string>("tooltip");
        TabParameters.BadgeText = Parameters.GetValue<string>("badgeText");
        TabParameters.BadgeClasses = Parameters.GetValue<string>("badgeClasses") ?? TabParameters.BadgeClasses;
    }

    /// <summary>
    /// Handle the zone component with tab-specific logic
    /// </summary>
    public override async Task<ZoneComponentResult> HandleAsync(TModel model, ZoneRenderingContext context, TContext componentContext, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        // Update tab parameters from component parameters
        UpdateTabParameters();

        // Call the tab-specific handler
        var result = await HandleTabAsync(model, context, componentContext, parameters, cancellationToken);

        // If the result is not already a TabContentResult, wrap it
        if (result is not TabContentResult tabResult)
        {
            return TabContentResult.Html(result.HtmlContent ?? new HtmlString(""), TabParameters, result.ContinueProcessing, result.Parameters, result.Meta);
        }

        return tabResult;
    }

    /// <summary>
    /// Handle the tab component with tab-specific logic and context
    /// Override this method to implement tab-specific behavior
    /// </summary>
    /// <param name="model">The strongly typed input model</param>
    /// <param name="context">The zone rendering context</param>
    /// <param name="componentContext">The additional component context</param>
    /// <param name="parameters">The component parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tab content result</returns>
    public abstract Task<TabContentResult> HandleTabAsync(TModel model, ZoneRenderingContext context, TContext componentContext, ZoneComponentParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Render tab header HTML
    /// </summary>
    protected virtual string RenderTabHeader()
    {
        var activeClass = TabParameters.IsActive ? "active" : "";
        var disabledClass = TabParameters.IsDisabled ? "disabled" : "";
        var classes = $"nav-link {activeClass} {disabledClass} {TabParameters.CssClasses}".Trim();

        var attributes = string.Join(" ", TabParameters.Attributes.Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""));
        if (!string.IsNullOrEmpty(attributes))
            attributes = " " + attributes;

        var tooltipAttr = !string.IsNullOrEmpty(TabParameters.Tooltip) ? $" title=\"{TabParameters.Tooltip}\"" : "";
        var dataAttributes = TabParameters.LazyLoad ? " data-bs-toggle=\"tab\" data-lazy-load=\"true\"" : " data-bs-toggle=\"tab\"";

        var iconHtml = !string.IsNullOrEmpty(TabParameters.TabIcon) ? $"<i class=\"{TabParameters.TabIcon}\"></i> " : "";
        var badgeHtml = !string.IsNullOrEmpty(TabParameters.BadgeText) ? $" <span class=\"{TabParameters.BadgeClasses}\">{TabParameters.BadgeText}</span>" : "";

        return $"<li class=\"nav-item\" role=\"presentation\">" +
               $"<button class=\"{classes}\" id=\"{TabParameters.TabId}-tab\" data-bs-target=\"#{TabParameters.TabId}\" type=\"button\" role=\"tab\"{tooltipAttr}{dataAttributes}{attributes}>" +
               $"{iconHtml}{TabParameters.TabText}{badgeHtml}" +
               $"</button>" +
               $"</li>";
    }

    /// <summary>
    /// Render tab content HTML
    /// </summary>
    protected virtual string RenderTabContent(string content)
    {
        var activeClass = TabParameters.IsActive ? "show active" : "";
        var lazyLoadAttr = TabParameters.LazyLoad ? " data-lazy-loaded=\"false\"" : "";

        return $"<div class=\"tab-pane fade {activeClass}\" id=\"{TabParameters.TabId}\" role=\"tabpanel\" aria-labelledby=\"{TabParameters.TabId}-tab\"{lazyLoadAttr}>" +
               $"<div class=\"tab-content-wrapper\">" +
               $"{content}" +
               $"</div>" +
               $"</div>";
    }

    /// <summary>
    /// Render complete tab structure
    /// </summary>
    protected virtual string RenderCompleteTab(string content)
    {
        return RenderTabHeader() + "\n" + RenderTabContent(content);
    }

    /// <summary>
    /// Create a tab content result with rendered tab structure
    /// </summary>
    protected TabContentResult CreateTabResult(string content, bool continueProcessing = true, ZoneComponentParameters? parameters = null)
    {
        var htmlContent = new HtmlString(content);
        return TabContentResult.Html(htmlContent, TabParameters, continueProcessing, parameters);
    }

    /// <summary>
    /// Create a regular content result (not as a tab)
    /// </summary>
    protected TabContentResult CreateRegularContentResult(string content, bool continueProcessing = true, ZoneComponentParameters? parameters = null)
    {
        var htmlContent = new HtmlString(content);
        return TabContentResult.RegularContent(htmlContent, continueProcessing, parameters);
    }
} 