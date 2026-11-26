using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.WarehouseVendors
{
    public record WarehouseVendorModel : BaseNopEntityModel
    {
        public WarehouseVendorModel()
        {
            AvailableWarehouses = new();
            AvailableVendors = new();
        }

        public int VendorId { get; set; }

        public int WarehouseId { get; set; }

        public string VendorName { get; set; }

        public string WarehouseName { get; set; }

        public List<SelectListItem> AvailableWarehouses { get; set; }

        public List<SelectListItem> AvailableVendors { get; set; }

    }
}
