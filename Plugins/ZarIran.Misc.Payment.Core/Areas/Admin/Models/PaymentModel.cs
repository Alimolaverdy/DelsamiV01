using ZarIran.Plugin.Payment.Core.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace ZarIran.Plugin.Payment.Core.Areas.Admin.Models
{
    public record PaymentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.SourceType")]
        public SourceType SourceType { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Gateway")]
        public PaymentGateway Gateway { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Gateway")]
        public string GatewayStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.TeminalNo")]
        public string TeminalNo { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.TrackId")]
        public string TrackId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.MaskedCardNumber")]
        public string MaskedCardNumber { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.HashedCardNumber")]
        public string HashedCardNumber { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.UrlId")]
        public string UrlId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Amount")]
        public decimal Amount { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Amount")]
        public string AmountStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.AdditionalData")]
        public string AdditionalData { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime")]
        public string CreatedDateTimeStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime")]
        public DateTime? VerifyDateTime { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime")]
        public string VerifyDateTimeStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime")]
        public DateTime? ConfirmDateTime { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime")]
        public string ConfirmDateTimeStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Status")]
        public PaymentStatus Status { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Status")]
        public string StatusStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ResultCode")]
        public string ResultCode { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Completed")]
        public bool Completed { get; set; }

    }
}
