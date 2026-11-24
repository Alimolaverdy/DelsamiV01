using Nop.Web.Framework.Models;
using System;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;

public record ImportedTrackNumberReportModel:BaseNopModel
{
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string ErrorDescription { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

