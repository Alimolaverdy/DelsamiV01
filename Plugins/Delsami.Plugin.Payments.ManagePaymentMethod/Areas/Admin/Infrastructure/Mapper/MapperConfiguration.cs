using AutoMapper;
using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Infrastructure.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateCustomerCarMap();
        }


        private void CreateCustomerCarMap()
        {
            CreateMap<PaymentMethodDiscount, PaymentMethodDiscountModel>()
                .ForMember(dest => dest.StartDateStr, cfg => cfg.MapFrom(src => DateTimeHelper.GetPersianStrFromDateTime(src.StartDateTimeOnUtc.ToLocalTime(), true)))
                .ForMember(dest => dest.EndDateStr, cfg => cfg.MapFrom(src => DateTimeHelper.GetPersianStrFromDateTime(src.EndDateTimeOnUtc.HasValue ? src.EndDateTimeOnUtc.Value.ToLocalTime() : null, true)));
            CreateMap<PaymentMethodDiscountModel, PaymentMethodDiscount>();
        }

        public int Order => 1;

    }
}
