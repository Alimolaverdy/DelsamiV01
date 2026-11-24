using AutoMapper;
using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Mappers
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {

        public MapperConfiguration()
        {
            CreatePriceRangeMap();
        }

        private void CreatePriceRangeMap()
        {
            CreateMap<PriceRange, PriceRangeModel>()
                .ReverseMap();
        }

        public int Order => 1;

    }
}
