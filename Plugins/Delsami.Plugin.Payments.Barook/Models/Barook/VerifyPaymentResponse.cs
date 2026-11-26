using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Delsami.Plugin.Payment.Core.Models;

namespace Delsami.Plugin.Payments.Barook.Models.Barook
{
	public class VerifyPaymentResponse : BasePaymentResponseModel
	{
		[JsonProperty("totalAmount")]
		public decimal TotalAmount { get; set; }

		[JsonProperty("externalCode")]
		public string ExternalCode { get; set; }

		[JsonProperty("referenceNumber")]
		public string ReferenceNumber { get; set; }

		[JsonProperty("ownerName")]
		public string OwnerName { get; set; }

		[JsonProperty("ownerMobile")]
		public string OwnerMobile { get; set; }

		[JsonProperty("ownerNationalCode")]
		public string OwnerNationalCode { get; set; }

		[JsonProperty("status")]
		public TransactionStatusEnum Status { get; set; }

		[JsonProperty("saleItemDto")]
		public List<SaleItemDto> SaleItemDto { get; set; }

		[JsonProperty("branchCode")]
		public string BranchCode { get; set; }

		[JsonProperty("terminalNumber")]
		public string terminalNumber { get; set; }

		[JsonProperty("terminalCode")]
		public string TerminalCode { get; set; }

		[JsonProperty("details")]
		public Dictionary<string, object> Attributes { get; set; }


		public override string ToString()
		{
			return $"TotalAmount: {TotalAmount} - ExternalCode: {ExternalCode} - ReferenceNumber: {ReferenceNumber}" +
				   $" - OwnerName: {OwnerName} - OwnerMobile: {OwnerMobile} - OwnerNationalCode: {OwnerNationalCode}" +
				   $" - Status: {Status} - BranchCode: {BranchCode} - terminalNumber: {terminalNumber} - TerminalCode: {TerminalCode}";
		}
	}


}
