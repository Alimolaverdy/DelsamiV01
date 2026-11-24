using System;
using System.Collections.Generic;
using System.Text;

namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays
{
    public class CreateTicketResponseModel
    {
        public string redirectUrl { get; set; }

        public string ticket { get; set; }

        public ResultModel result { get; set; }
    }
}
