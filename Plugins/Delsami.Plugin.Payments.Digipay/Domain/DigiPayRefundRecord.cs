using System;
using Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund;
using Nop.Core;

namespace Delsami.Plugin.Payments.Digipay.Domain
{
	public class DigiPayRefundRecord : BaseEntity
	{
		public string Customer { get; set; }

		public string PaymentTrackingCode { get; set; }

		public string RefundTrackingCode { get; set; }

		public string Amount { get; set; }

		public string Message { get; set; }

		public PaymentType PaymentType { get; set; }

		public bool Status { get; set; }

		public DateTime RefundDateTimeOnUtc { get; set; }
	}
}
