using Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;
using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.ShipmentManagement.Domain;

public class ShipmentTrackingNumberImporterReport : BaseEntity
{
    public int ShipmentTrackingNumberImportId { get; set; }
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public string TrackingNumber { get; set; }
    public int Status { get; set; } 
    public string ErrorDescription { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public int CustomerId { get; set; } 
}
