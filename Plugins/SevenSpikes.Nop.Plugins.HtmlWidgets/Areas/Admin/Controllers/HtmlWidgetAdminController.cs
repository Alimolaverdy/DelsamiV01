using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using SevenSpikes.Nop.Conditions.Services;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Models;
using SevenSpikes.Nop.Mappings.Services;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Services;
using SevenSpikes.Nop.Scheduling.Services;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.HtmlWidgets", false)]
public class HtmlWidgetAdminController : Base7SpikesAdminController
{
    private readonly ILogger _logger;
    private readonly IHtmlWidgetService _htmlWidgetService;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly ILocalizedEntityService _localizedEntityService;
    private readonly ISchedulingService _schedulingService;
    private readonly IConditionService _conditionService;
    private readonly IEntityMappingService _entityMappingService;
    private readonly IEntityWidgetMappingService _entityWidgetMappingService;

    public HtmlWidgetAdminController(
        ILogger logger,
        IHtmlWidgetService htmlWidgetService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        ISchedulingService schedulingService,
        IConditionService conditionService,
        IEntityMappingService entityMappingService,
        IEntityWidgetMappingService entityWidgetMappingService)
    {
        _logger = logger;
        _htmlWidgetService = htmlWidgetService;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _schedulingService = schedulingService;
        _conditionService = conditionService;
        _entityMappingService = entityMappingService;
        _entityWidgetMappingService = entityWidgetMappingService;
    }

    public ActionResult List()
    {
        return View("List", new HtmlWidgetSearchModel());
    }

    [HttpPost]
    public async Task<ActionResult> List(HtmlWidgetSearchModel searchModel)
    {
        var models = await PrepareListModelAsync(searchModel);
        var pagedList = new PagedList<HtmlWidgetModel>(
            models,
            searchModel.Page - 1,
            searchModel.PageSize,
            null);

        var listModel = new HtmlWidgetListModel();
        listModel.PrepareToGrid(searchModel, pagedList, () => pagedList);

        return Json(listModel);
    }

    public async Task<ActionResult> Create()
    {
        var model = new HtmlWidgetModel
        {
            IsTrialVersion = false
        };

        await AddLocalesAsync<HtmlWidgetLocalizedModel>(_languageService, model.Locales);
        return View("Create", model);
    }

    private async Task<List<HtmlWidgetModel>> PrepareListModelAsync(HtmlWidgetSearchModel searchModel)
    {
        var widgets = await _htmlWidgetService.GetAllHtmlWidgetsAsync();
        var models = new List<HtmlWidgetModel>(widgets.Count);

        foreach (var widget in widgets)
        {
            var model = widget.ToModel();
            await AddLocalesAsync(widget, model);
            models.Add(model);
        }

        if (!string.IsNullOrEmpty(searchModel.SearchWidgetName))
        {
            models = models
                .Where(model => model.Name != null && model.Name.Contains(searchModel.SearchWidgetName))
                .ToList();
        }

        if (!string.IsNullOrEmpty(searchModel.SearchWidgetContent))
        {
            models = models
                .Where(model => model.UseJavaScript
                    ? model.JavaScriptContent != null
                      && model.JavaScriptContent.Contains(searchModel.SearchWidgetContent)
                    : model.HtmlContent != null
                      && model.HtmlContent.Contains(searchModel.SearchWidgetContent))
                .ToList();
        }

        return models;
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<ActionResult> Create(HtmlWidgetModel model, bool continueEditing)
    {
        if (!ModelState.IsValid)
            return View("Create", model);

        var htmlWidget = model.ToEntity();
        await _htmlWidgetService.InsertHtmlWidgetAsync(htmlWidget);
        await UpdateLocalesAsync(htmlWidget, model);

        SuccessNotification(await _localizationService.GetResourceAsync(
            "SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.SuccessfullyCreated"));

        if (!continueEditing)
            return RedirectToAction(nameof(List));

        SaveSelectedTabName(string.Empty, true);
        return RedirectToAction(nameof(Edit), new { id = htmlWidget.Id });
    }

    public async Task<ActionResult> Edit(int id)
    {
        var htmlWidget = await _htmlWidgetService.GetHtmlWidgetByIdAsync(id);
        if (htmlWidget == null)
            return NotFound();

        var model = htmlWidget.ToModel();
        await AddLocalesAsync(htmlWidget, model);
        PrepareCopyModel<HtmlWidget>(
            model.CopyModel,
            htmlWidget,
            "Copy",
            "HtmlWidgetAdmin",
            $"Copy of {htmlWidget.Name}",
            true,
            true,
            true);
        await PrepareStoresMappingModelAsync<HtmlWidget>(
            model.StoreMappingModel,
            htmlWidget,
            excludeProperties: false);

        model.IsTrialVersion = false;
        return View("Edit", model);
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    public Task<ActionResult> Edit(HtmlWidgetModel model, bool continueEditing)
    {
        return EditInternalAsync(model, continueEditing);
    }

    private async Task<ActionResult> EditInternalAsync(HtmlWidgetModel model, bool continueEditing)
    {
        if (!ModelState.IsValid)
            return View("Edit", model);

        var htmlWidget = await _htmlWidgetService.GetHtmlWidgetByIdAsync(model.Id);
        if (htmlWidget == null)
            throw new ArgumentException("No HTML widget found with the specified id", nameof(model));

        htmlWidget = model.ToEntity(htmlWidget);
        await SaveStoreMappingsAsync<HtmlWidget>(htmlWidget, model.StoreMappingModel);
        await _htmlWidgetService.UpdateHtmlWidgetAsync(htmlWidget);
        await UpdateLocalesAsync(htmlWidget, model);

        SuccessNotification(await _localizationService.GetResourceAsync(
            "SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.SuccessfullyEdited"));

        if (!continueEditing)
            return RedirectToAction(nameof(List));

        SaveSelectedTabName(string.Empty, true);
        return RedirectToAction(nameof(Edit), new { id = htmlWidget.Id });
    }

    public async Task<ActionResult> DeleteHtmlWidget(int id)
    {
        await DeleteConfirmedAsync(id);
        return EmptyJson;
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await DeleteConfirmedAsync(id);
        return RedirectToAction(nameof(List));
    }

    private async Task DeleteConfirmedAsync(int id)
    {
        var htmlWidget = await _htmlWidgetService.GetHtmlWidgetByIdAsync(id);
        if (htmlWidget == null)
            throw new ArgumentException("No HTML widget found with the specified id", nameof(id));

        await _htmlWidgetService.DeleteHtmlWidgetAsync(htmlWidget);
        SuccessNotification(await _localizationService.GetResourceAsync(
            "SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.SuccessfullyDeleted"));
    }

    private async Task AddLocalesAsync(
        HtmlWidget htmlWidget,
        ILocalizedModel<HtmlWidgetLocalizedModel> localizedModel)
    {
        await AddLocalesAsync<HtmlWidgetLocalizedModel>(_languageService, localizedModel.Locales);

        foreach (var locale in localizedModel.Locales)
        {
            locale.HtmlContent = await _localizationService.GetLocalizedAsync(
                htmlWidget,
                widget => widget.HtmlContent,
                locale.LanguageId,
                returnDefaultValue: false,
                ensureTwoPublishedLanguages: false);
        }
    }

    [NonAction]
    private async Task UpdateLocalesAsync(
        HtmlWidget htmlWidget,
        ILocalizedModel<HtmlWidgetLocalizedModel> localizedModel)
    {
        foreach (var locale in localizedModel.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(
                htmlWidget,
                widget => widget.HtmlContent,
                locale.HtmlContent,
                locale.LanguageId);
        }
    }

    public async Task<ActionResult> Copy(CopyModel model)
    {
        var originalWidget = await _htmlWidgetService.GetHtmlWidgetByIdAsync(model.Id);
        var originalWidgetId = originalWidget?.Id ?? model.Id;

        try
        {
            if (originalWidget == null)
                throw new ArgumentException("No HTML widget found with the specified id", nameof(model));

            var copy = await CopyEntityAsync(
                originalWidget,
                model.Name,
                model.CopyConditions,
                model.CopyMappings,
                model.CopyScheduling);

            SuccessNotification("The HTML widget has been copied successfully.");
            return RedirectToAction(nameof(Edit), new { id = copy.Id });
        }
        catch (Exception exception)
        {
            var errorMessage = $"An error occurred while copying HTML widget #{originalWidgetId}. Check the system log for details.";
            await _logger.ErrorAsync(errorMessage, exception);
            ErrorNotification(errorMessage);
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }
    }

    private async Task<HtmlWidget> CopyEntityAsync(
        HtmlWidget originalWidget,
        string newName,
        bool copyConditions,
        bool copyMappings,
        bool copyScheduling)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("An HTML widget name is required.", nameof(newName));

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var copy = new HtmlWidget
        {
            Name = newName,
            Visible = originalWidget.Visible,
            HtmlContent = originalWidget.HtmlContent,
            LimitedToStores = originalWidget.LimitedToStores
        };

        await _htmlWidgetService.InsertHtmlWidgetAsync(copy);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
        foreach (var language in languages)
        {
            var localizedContent = await _localizationService.GetLocalizedAsync(
                originalWidget,
                widget => widget.HtmlContent,
                language.Id,
                returnDefaultValue: false,
                ensureTwoPublishedLanguages: false);

            if (!string.IsNullOrEmpty(localizedContent))
            {
                await _localizedEntityService.SaveLocalizedValueAsync(
                    copy,
                    widget => widget.HtmlContent,
                    localizedContent,
                    language.Id);
            }
        }

        var storeIds = await StoreMappingService.GetStoresIdsWithAccessAsync(originalWidget);
        foreach (var storeId in storeIds)
            await StoreMappingService.InsertStoreMappingAsync(copy, storeId);

        if (copyScheduling)
        {
            await _schedulingService.CopyScheduleAsync(
                Plugin.EntityType,
                originalWidget.Id,
                copy.Id);
        }

        if (copyConditions)
        {
            await _conditionService.CopyConditionAsync(
                Plugin.EntityType,
                originalWidget.Id,
                copy.Id);
        }

        if (copyMappings)
        {
            await _entityMappingService.CopyEntityMappingsAsync(
                Plugin.EntityType,
                originalWidget.Id,
                copy.Id);
            await _entityWidgetMappingService.CopyEntityWidgetMappingsAsync(
                Plugin.EntityType,
                originalWidget.Id,
                copy.Id);
        }

        transaction.Complete();
        return copy;
    }
}
