namespace Delsami.Plugin.Payments.TorobPay.Services.Dtos
{
    public class ErrorDto
    {
        public class ErrorRoot
        {
            public bool successful { get; set; }
            public TorobError error { get; set; }
        }

        public class TorobError
        {
            public int code { get; set; }
            public string message { get; set; }
            public string user_message { get; set; }
            public object data { get; set; }
        }
    }
}
