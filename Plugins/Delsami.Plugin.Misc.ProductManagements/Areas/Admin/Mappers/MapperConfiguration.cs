using AutoMapper;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.Products;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Mappers
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateZarIranProductMap();
        }

        public int Order => 1;

        private void CreateZarIranProductMap()
        {
            CreateMap<Product, ZarIranProductModel>()
                .ReverseMap();
        }

    }
}
