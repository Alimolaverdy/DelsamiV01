using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.ActionFilters;

public class SearchActionFilterAttribute : ActionFilterAttribute
{
    private NopAjaxFiltersSettings _ajaxFiltersSettings;

    private WidgetSettings _widgetSettings;

    private IUrlHelperFactory _urlHelperFactory;

    private NopAjaxFiltersSettings AjaxFiltersSettings => _ajaxFiltersSettings ?? (_ajaxFiltersSettings = EngineContext.Current.Resolve<NopAjaxFiltersSettings>());

    private WidgetSettings WidgetSettings => _widgetSettings ?? (_widgetSettings = EngineContext.Current.Resolve<WidgetSettings>());

    private IUrlHelperFactory UrlHelperFactory => _urlHelperFactory ?? (_urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>());

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (AjaxFiltersSettings.EnableAjaxFilters && WidgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxFilters") && !string.IsNullOrEmpty(AjaxFiltersSettings.WidgetZone) && AjaxFiltersSettings.ShowFiltersOnSearchPage)
        {
            string str = UrlHelperFactory.GetUrlHelper(filterContext).RouteUrl("FilterProductSearch") + filterContext.HttpContext.Request.QueryString.ToUriComponent();
            filterContext.HttpContext.Response.StatusCode = 302;
            filterContext.HttpContext.Response.Headers["Location"] = str;
        }
        base.OnActionExecuting(filterContext);
    }
}
