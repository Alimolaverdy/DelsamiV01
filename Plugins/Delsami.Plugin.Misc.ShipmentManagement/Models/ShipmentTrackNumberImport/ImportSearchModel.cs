using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;

public record ImportSearchModel: BaseSearchModel
{
    [NopResourceDisplayName("Admin.Orders.Shipments.ImportShipmentTrackNumber.StartDate")]
    [UIHint("Date")]
    public DateTime? StartDate { get; set; } = DateTime.Now;

    [NopResourceDisplayName("Admin.Orders.Shipments.ImportShipmentTrackNumber.EndDate")]
    [UIHint("Date")]
    public DateTime? EndDate { get; set; } = DateTime.Now;
}

