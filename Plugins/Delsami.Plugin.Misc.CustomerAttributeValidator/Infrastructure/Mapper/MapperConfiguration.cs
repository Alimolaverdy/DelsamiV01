using AutoMapper;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Infrastructure.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateCustomerAttributeValidationMap();
        }

        private void CreateCustomerAttributeValidationMap()
        {
            CreateMap<CustomerAttributeValidation, CustomerAttributeValidationModel>()
                .ReverseMap();
        }
    }
}
