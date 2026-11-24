using AutoMapper;
using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers;
using Nop.Core.Infrastructure.Mapper;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {

        public MapperConfiguration()
        {
            CreateCustomerShebaNumberMap();
        }

        private void CreateCustomerShebaNumberMap()
        {
            CreateMap<CustomerShebaNumber, CustomerShebaNumberModel>()
                .ReverseMap();
        }
        public int Order => 1;

    }
}
