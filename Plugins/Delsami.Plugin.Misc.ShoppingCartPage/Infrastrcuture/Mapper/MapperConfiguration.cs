using AutoMapper;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Misc.CarInfo.Infrastrcuture.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {

        public MapperConfiguration()
        {
            //CreateCustomerCarMap();
            //CreateCarServiceReminderMap();
            //CreateManufacturerModelSpecificationMap();            //CreateCustomerCarMap();
            //CreateCarServiceReminderMap();
            //CreateManufacturerModelSpecificationMap();
        }

        //private void CreateCustomerCarMap()
        //{
        //    CreateMap<CustomerCar, CustomerCarModel>()
        //        .ReverseMap();
        //}

        //private void CreateCarServiceReminderMap()
        //{
        //    CreateMap<CarServiceReminder, CarServiceReminderModel>()
        //        .ForMember(dest => dest.CarServiceReminderStatusStr, cfg => cfg.MapFrom(src => EnumHelper.GetEnumDescription(src.CarServiceReminderStatus)))
        //        .ForMember(dest => dest.CarServiceReminderTypeStr, cfg => cfg.MapFrom(src => EnumHelper.GetEnumDescription(src.CarServiceReminderType)));

        //    CreateMap<CarServiceReminderModel, CarServiceReminder>();
        //}

        //private void CreateManufacturerModelSpecificationMap()
        //{
        //    CreateMap<ManufacturerModelSpecificationOption, ManufacturerModelSpecificationOptionModel>()
        //         .ReverseMap();
        //}

        public int Order => 1;

    }
}
