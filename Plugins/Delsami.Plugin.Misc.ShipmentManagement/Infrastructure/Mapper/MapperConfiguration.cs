using AutoMapper;
using Delsami.Plugin.Misc.ShipmentManagement.Areas.Admin.Models.Shipments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Misc.ShipmentManagement.Infrastructure.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {

        public MapperConfiguration()
        {
            CreateCustomerCarMap();
        }

        public int Order => 1;

        private void CreateCustomerCarMap()
        {
            CreateMap<Shipment, DelsamiShipmentModel>();
        }
    }
}
