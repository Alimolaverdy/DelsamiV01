using AutoMapper;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.BundleProducts;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.WarehouseVendors;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Core.Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Infrastructure.Mappers
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateWarehouseVendorMap();
            CreateBundleProductMap();
        }

        private void CreateWarehouseVendorMap()
        {
            CreateMap<WarehouseVendor, WarehouseVendorModel>()
                .ReverseMap();
        }

        private void CreateBundleProductMap()
        {
            CreateMap<BundleProduct, BundleProductModel>()
                .ReverseMap();
        }

        public int Order => 1;
    }
}
