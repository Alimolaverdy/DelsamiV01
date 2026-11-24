using Nop.Core;
using System;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Domain;

public class ShipmentTrackingNumberImport: BaseEntity
{
    public byte[] BinaryData { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
