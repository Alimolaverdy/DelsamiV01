using Delsami.Plugin.Payment.Core.Models;
using System.Collections.Generic;

namespace Delsami.Plugin.Payments.TorobPay.Services.Dtos
{
    public class PaymentDto
    {
        public class PaymentTokenRequest
        {
            public decimal amount { get; set; }
            public string paymentMethodTypeDto { get; set; } = "ONLINE_CREDIT";
            public string returnURL { get; set; }
            public string transactionId { get; set; }
            public string mobile { get; set; }
            public List<Cart> cartList { get; set; }
        }

        public class Cart
        {
            public string cartId { get; set; }
            public decimal totalAmount { get; set; }
            public List<CartItem> cartItems { get; set; }
        }

        public class CartItem
        {
            public string id { get; set; }
            public string name { get; set; }
            public int count { get; set; }
            public decimal amount { get; set; }
            public string category { get; set; } = "کالا";
        }

        public class PaymentTokenResponseRoot
        {
            public bool successful { get; set; }
            public PaymentTokenResponse response { get; set; }
        }

        public class PaymentTokenResponse
        {
            public string paymentToken { get; set; }
            public string paymentPageUrl { get; set; }
        }

        public class VerifyRequest { public string paymentToken { get; set; } }
        public class VerifyResponseRoot
        {
            public bool successful { get; set; }
            public VerifyResponse response { get; set; }
        }
        public class VerifyResponse : BasePaymentResponseModel
        {
            public string transactionId { get; set; }
            public override string ToString()
            {
                return $"transactionId: {transactionId}";
            }
        }

        public class SettleRequest { public string paymentToken { get; set; } }
        public class SettleResponseRoot
        {
            public bool successful { get; set; }
            public SettleResponse response { get; set; }
        }
        public class SettleResponse : BasePaymentResponseModel
        {
            public string transactionId { get; set; }

            public override string ToString()
            {
                return $"transactionId: {transactionId}";
            }
        }

        public class StatusResponseRoot
        {
            public bool successful { get; set; }
            public StatusResponse response { get; set; }
        }
        public class StatusResponse
        {
            public string status { get; set; }
            public int amount { get; set; }
            public string transactionId { get; set; }
        }

        public class CancelRequest { public string paymentToken { get; set; } }
        public class UpdateRequest : PaymentTokenRequest { public string paymentToken { get; set; } }
    }
}
