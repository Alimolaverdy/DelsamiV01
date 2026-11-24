using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NopFarsi.Payments.IranKish
{
    public class IkcSettings : ISettings
    {

        public string MerchantId
        {
            get;
            set;
        }


        public string WebServiceUrl
        {
            get { return ""; }

            //set;
        }

        public bool IsToman
        {
            get;
            set;
        }

        public bool DisablePaymentInfo
        {
            get;
            set;
        }

        public string Apportionment { get; set; }

    }
}
