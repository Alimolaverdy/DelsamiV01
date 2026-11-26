using AutoMapper;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Mappers
{
    public class OrderSubStatusMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public OrderSubStatusMapperConfiguration()
        {
            CreateCustomerCarMap();
        }

        private void CreateCustomerCarMap()
        {
            CreateMap<OrderSubStatus, OrderSubStatusModel>()
                .ReverseMap();
        }

        public int Order => 1;
    }
}
