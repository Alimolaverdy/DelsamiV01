using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Products;

public record ImportReportListModel : BasePagedListModel<ImportReportModel> { }


public record ImportReportModel : BaseNopModel {

    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string ImportType { get; set; }
    public string CreatedOn { get; set; }

}


public record ImportReportSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Delsami.Misc.ProductManagement.ImportList.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; } 

    [NopResourceDisplayName("Delsami.Misc.ProductManagement.ImportList.Startdate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Delsami.Misc.ProductManagement.ImportList.AvailableImportTypes")]
    public int ImportTypeId { get; set; }

    public IList<SelectListItem> AvailableImportTypes { get; set; } = new List<SelectListItem>();

}
