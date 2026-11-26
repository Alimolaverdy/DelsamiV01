using AutoMapper;
using Delsami.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Shipping.IRPost.Areas.Admin.Mappers
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
