using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;

public record ReportSearchModel: BaseSearchModel
{
    public int ShipmentImportId { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.ReportSearchModel.Fields.Status")]
    public int Status { get; set; }
    public IList<SelectListItem> AvailableStatuses { get; set; } = new List<SelectListItem>();
}

