using Nop.Web.Framework.Models;
using System;

namespace Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;

public record ShipmentImportModel:BaseNopModel
    {
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

