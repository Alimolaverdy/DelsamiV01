using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using ZarIran.Plugin.Misc.ShipmentManagement.Models;
using ZarIran.Plugin.Misc.ShipmentManagement.Models.Apis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Services
{
    public interface IShipmentDeliveryService
    {
        Task<ShipmentDelivery> GetShipmentDeliveryByShipmentIdAsync(int shipmentId);

        Task<(ZarIranOrderDetailModel Result, string ErrorMessage)> GetOrderDetailsByShipmentIdAsync(string shipmentCode);

        Task InsertAsync(ShipmentDelivery shipmentAdditionalData);

        Task<bool> CheckDeliveryCodeIsDuplicated(int deliveryCode);

        Task<(bool Result, string ErrorMessage)> ValidateDeliveryCodeAsync(string shipmentCode, int deliveryCode);

        Task<(bool Result, string ErrorMessage)> CheckDeliveryCodeAsync(string shipmentCode, int deliveryCode);

        Task<(bool Result, string ErrorMessage)> ShippedAsync(string shipmentCode);

        Task<(bool Result, string ErrorMessage)> InsertPaymentAsync(InsertPaymentRequestModel paymentRequest);

        Task<(bool Result, string ErrorMessage)> ValidateCODDeliveryCodeAsync(bool hasDeliveryCode, int? deliveryCode, string shipmentCode);

        Task<(bool Result, string ErrorMessage, List<Payment.Core.Domain.Payment> Payments)> InsertCODPaymentsAsync(InsertCODPaymentsRequestModel paymentsRequest);

        Task<(bool Result, string ErrorMessage, Payment.Core.Domain.Payment Payment)> InsertCODPaymentAsync(InsertCODPaymentRequestModel paymentRequest);

    }
}
