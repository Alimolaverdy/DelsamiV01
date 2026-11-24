using AutoMapper;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.WarehouseVendors;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace ZarIran.Plugin.Misc.ProductManagements.Infrastructure.Mappers
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateWarehouseVendorMap();
            CreateOtherBrandMap();
            CreateBundleProductMap();
            CreateBundleProductMap();
        }

        private void CreateWarehouseVendorMap()
        {
            CreateMap<WarehouseVendor, WarehouseVendorModel>()
                .ReverseMap();
        }

        private void CreateOtherBrandMap()
        {
            CreateMap<OtherBrandProduct, OtherBrandProductModel>()
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
