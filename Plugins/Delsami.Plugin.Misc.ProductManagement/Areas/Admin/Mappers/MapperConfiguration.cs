using AutoMapper;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Products;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Mappers
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateDelsamiProductMap();
        }

        public int Order => 1;

        private void CreateDelsamiProductMap()
        {
            CreateMap<Product, DelsamiProductModel>()
                .ReverseMap();
        }
    }
}
