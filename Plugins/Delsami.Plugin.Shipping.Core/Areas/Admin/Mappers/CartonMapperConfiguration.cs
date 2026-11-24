using AutoMapper;
using ZarIran.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using ZarIran.Plugin.Shipping.Core.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace ZarIran.Plugin.Shipping.Core.Areas.Admin.Mappers
{
    public class CartonMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public CartonMapperConfiguration()
        {
            CreateCustomerCarMap();
        }

        private void CreateCustomerCarMap()
        {
            CreateMap<Carton, CartonModel>()
                .ReverseMap();
        }

        public int Order => 1;

    }
}
