using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;
using Nop.Plugin.Misc.AdvRedirect.Infrastructure.Permissions;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class RedirectionRuleController : BasePluginController
{

    #region Fields

    private readonly IRedirectionsService _redirectionsService;
    private readonly IDataIOService _dataIoService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<UrlRecord> _urlRecordRepo;

    #endregion

    #region Ctor

    public RedirectionRuleController(IRedirectionsService redirectionsService, IDataIOService dataIoService, INotificationService notificationService,
                                        IPermissionService permissionService, IRepository<UrlRecord> urlRecordRepo, IRepository<Product> productService)
    {
        _redirectionsService = redirectionsService;
        _dataIoService = dataIoService;
        _urlRecordRepo = urlRecordRepo;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _productRepo = productService;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> GetRedirections(RedirectionSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            return AccessDeniedView();

        var data = await _redirectionsService.GetAllRedirectionsAsync(searchModel);

        var model = new RedirectionsListModel().PrepareToGrid(searchModel, data, () =>
        {
            return data.Select(l => l.ToModel<RedirectionModel>());
        });

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> RedirectAdd(RedirectionModel model)
    {
        if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var result = await _redirectionsService.InsertRedirectionsAsync(model.ToEntity<RedirectionRule>());
        if (result != InsertRedirectionResult.Ok)
            return ErrorJson($"{result}");

        return Json(new { Result = true });
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            return AccessDeniedView();

        var model = new ConfigurationModel()
        {
            SearchModel = new RedirectionSearchModel() { AvailablePageSizes = "10,20,30" }
        };

        foreach (var r in (RedirectionTypeEnum[])Enum.GetValues(typeof(RedirectionTypeEnum)))
        {
            model.AvailableTypes.Add(new SelectListItem() { Text = r.ToString(), Selected = !model.AvailableTypes.Any(), Value = r.ToString() });
        }

        return View("~/Plugins/Misc.AdvRedirect/Areas/Admin/Views/RedirectionRule/List.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            return AccessDeniedView();

        var bytes = await _dataIoService.ExportAsync();
        return File(bytes, "text/csv; charset=utf-8", "export.xlsx");

    }

    [HttpPost]
    public async Task<IActionResult> RedirectRemove(RedirectionModel model)
    {
        if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            return AccessDeniedView();

        await _redirectionsService.DeleteRedirectionAsync(model.ToEntity<RedirectionRule>());

        return Json(new { Result = true });
    }

    #endregion

}