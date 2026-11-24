using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using SevenSpikes.Nop.Conditions.Domain;
using SevenSpikes.Nop.Conditions.Domain.FakeEntity;
using SevenSpikes.Nop.Conditions.Helpers;
using SevenSpikes.Nop.Conditions.Services;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Mappings.Services;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Extensions;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Services;
using SevenSpikes.Nop.Scheduling.Domain;
using SevenSpikes.Nop.Scheduling.Helpers;
using SevenSpikes.Nop.Scheduling.Services;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Components;

[ViewComponent(Name = "HtmlWidget")]
public class HtmlWidgetComponent : ViewComponent
{
    private readonly IHtmlWidgetService _htmlWidgetService;
    private readonly IProductService _productService;
    private readonly IManufacturerService _manufacturerService;
    private readonly ICategoryService _categoryService;
    private readonly ITopicService _topicService;
    private readonly IVendorService _vendorService;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IWebHelper _webHelper;
    private readonly IConditionService _conditionService;
    private readonly IConditionChecker _conditionChecker;
    private readonly ISchedulingService _schedulingService;
    private readonly ISchedulingChecker _schedulingChecker;
    private readonly IEntityWidgetMappingService _entityWidgetMappingService;
    private readonly HtmlWidgetsSettings _htmlWidgetsSettings;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private IStaticCacheManager _staticCacheManager;

    private IStaticCacheManager StaticCacheManager =>
        _staticCacheManager ??= EngineContext.Current.Resolve<IStaticCacheManager>();

    public HtmlWidgetComponent(
        IHtmlWidgetService htmlWidgetService,
        IProductService productService,
        IManufacturerService manufacturerService,
        ICategoryService categoryService,
        ITopicService topicService,
        IVendorService vendorService,
        IWorkContext workContext,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IConditionService conditionService,
        IConditionChecker conditionChecker,
        ISchedulingService schedulingService,
        ISchedulingChecker schedulingChecker,
        IEntityWidgetMappingService entityWidgetMappingService,
        HtmlWidgetsSettings htmlWidgetsSettings,
        ILocalizationService localizationService,
        ICustomerService customerService)
    {
        _htmlWidgetService = htmlWidgetService;
        _productService = productService;
        _manufacturerService = manufacturerService;
        _categoryService = categoryService;
        _topicService = topicService;
        _vendorService = vendorService;
        _workContext = workContext;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _conditionService = conditionService;
        _conditionChecker = conditionChecker;
        _schedulingService = schedulingService;
        _schedulingChecker = schedulingChecker;
        _entityWidgetMappingService = entityWidgetMappingService;
        _htmlWidgetsSettings = htmlWidgetsSettings;
        _localizationService = localizationService;
        _customerService = customerService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone)
    {
        if (!_htmlWidgetsSettings.EnableHtmlWidgets)
            return Content(string.Empty);

        var productId = GetRouteId("productid");
        var categoryId = GetRouteId("categoryid");
        var manufacturerId = GetRouteId("manufacturerid");
        var topicId = GetRouteId("topicid");
        var vendorId = GetRouteId("vendorid");

        var widgetZones = await StaticCacheManager.GetAsync<IDictionary<string, bool>>(
            CacheKeys.HtmlWidgetsWidgetZonesDictionaryCacheKey,
            async () =>
            {
                var mappings = await _entityWidgetMappingService
                    .GetAllEntityWidgetMappingsByEntityTypeAsync(Plugin.EntityType);

                return mappings
                    .GroupBy(mapping => mapping.WidgetZone)
                    .ToDictionary(group => group.Key, _ => true);
            });

        if (!widgetZones.ContainsKey(widgetZone))
            return Content(string.Empty);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var isSecureConnection = _webHelper.IsCurrentConnectionSecured();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

        var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(
            CacheKeys.ProductHtmlWidgetsForWidgetCacheKey,
            customer.Id,
            customerRoleIds,
            productId,
            categoryId,
            manufacturerId,
            topicId,
            vendorId,
            widgetZone,
            languageId,
            storeId,
            isSecureConnection);

        var model = await StaticCacheManager.GetAsync(
            cacheKey,
            () => GetDefaultPublicModelsAsync(
                customer,
                productId,
                categoryId,
                manufacturerId,
                topicId,
                vendorId,
                widgetZone));

        return View("HtmlWidget", model);
    }

    private async Task<HtmlWidgetsPublicFullModel> GetDefaultPublicModelsAsync(
        Customer customer,
        int productId,
        int categoryId,
        int manufacturerId,
        int topicId,
        int vendorId,
        string widgetZone)
    {
        var widgets = await _htmlWidgetService.GetHtmlWidgetsByWidgetZoneAsync(widgetZone);
        var eligibleWidgets = new List<HtmlWidget>(widgets.Count);

        foreach (var widget in widgets)
        {
            if (await IsWidgetEligibleAsync(
                    widget,
                    customer,
                    productId,
                    categoryId,
                    manufacturerId,
                    topicId,
                    vendorId))
            {
                eligibleWidgets.Add(widget);
            }
        }

        var models = new List<HtmlWidgetsPublicModel>(eligibleWidgets.Count);
        foreach (var widget in eligibleWidgets)
            models.Add(await PrepareHtmlWidgetModelAsync(widget));

        return new HtmlWidgetsPublicFullModel
        {
            HtmlWidgets = models
        };
    }

    private async Task<bool> IsWidgetEligibleAsync(
        HtmlWidget widget,
        Customer customer,
        int productId,
        int categoryId,
        int manufacturerId,
        int topicId,
        int vendorId)
    {
        var schedule = await _schedulingService.GetScheduleByEntityTypeAndEntityIdAsync(
            Plugin.EntityType,
            widget.Id);

        if (schedule == null || !_schedulingChecker.Check(schedule))
            return false;

        var condition = await _conditionService.GetConditionByEntityTypeAndEntityIdAsync(
            Plugin.EntityType,
            widget.Id,
            true);

        if (condition == null)
            return false;

        var entities = new List<BaseEntity> { customer };

        if (productId != 0)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null)
            {
                entities.Add(product);
                entities.Add(new ProductSpecification(product));
            }
        }

        if (categoryId != 0)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category != null)
                entities.Add(category);
        }

        if (manufacturerId != 0)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
            if (manufacturer != null)
                entities.Add(manufacturer);
        }

        if (topicId != 0)
        {
            var topic = await _topicService.GetTopicByIdAsync(topicId);
            if (topic != null)
                entities.Add(topic);
        }

        if (vendorId != 0)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor != null)
                entities.Add(vendor);
        }

        return await _conditionChecker.CheckAsync(condition, entities);
    }

    private int GetRouteId(string routeKey)
    {
        var routeValue = RouteData.Values
            .FirstOrDefault(pair => string.Equals(pair.Key, routeKey, StringComparison.OrdinalIgnoreCase))
            .Value;

        return int.TryParse(Convert.ToString(routeValue), out var id) ? id : 0;
    }

    private async Task<HtmlWidgetsPublicModel> PrepareHtmlWidgetModelAsync(HtmlWidget htmlWidget)
    {
        var model = htmlWidget.ToPublicModel();
        model.HtmlContent = await _localizationService.GetLocalizedAsync(
            htmlWidget,
            widget => widget.HtmlContent);

        return model;
    }
}
