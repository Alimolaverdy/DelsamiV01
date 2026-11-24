using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace NopFarsi.Payments.IranKish.Models
{

    public class ConfigurationModel : BaseNopModel
    {

        public int ActiveStoreScopeConfiguration
        {
            get;
            set;
        }


        [NopResourceDisplayName("NopFarsi.Ikc.Fields.MerchantId")]
        public string MerchantId
        {
            get;
            set;
        }


        public bool MerchantIdOverrideForStore
        {
            get;
            set;
        }




        [NopResourceDisplayName("NopFarsi.Ikc.Fields.IsToman")]
        public bool IsTomanForStore
        {
            get;
            set;
        }


        [NopResourceDisplayName("NopFarsi.Ikc.Fields.DisablePaymentInfo")]
        public bool DisablePaymentInfoForStore
        {
            get;
            set;
        }

        public bool DisablePaymentInfo
        {
            get;
            set;
        }

        [NopResourceDisplayName("NopFarsi.Ikc.Fields.Apportionment")]
        public string Apportionment { get; set; }
        public bool ApportionmentForStore { get; set; }
    }
}
