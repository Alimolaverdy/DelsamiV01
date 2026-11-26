using System;
using System.Collections.Generic;
using System.Text;

namespace Delsami.Plugin.Payments.Digipay.Models.DigiPays
{
    public class ResultModel
    {
        public int status { get; set; }

        public string message { get; set; }

        public string level { get; set; }
    }
}
