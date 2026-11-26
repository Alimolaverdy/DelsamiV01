using Nop.Core.Configuration;

namespace Delsami.Plugin.Shipping.IRPost
{
    public class IRPostSettings : ISettings
    {

        public string User { get; set; }

        public string Password { get; set; }

        public int ContractCode { get; set; }

        public string PostNodeCode { get; set; }

        public string BaseUrl { get; set; }

        public string NeighbouringProvinces { get; set; }

        public bool EnableManualCalc { get; set; }

        //public decimal NeighbouringZeroToOneKgPostPrice { get; set; }
        //public decimal NonNeighbouringZeroToOneKgPostPrice { get; set; }

        //public decimal NeighbouringOneToTwoKgPostPrice { get; set; }
        //public decimal NonNeighbouringOneToTwoKgPostPrice { get; set; }

        //public decimal NeighbouringTwoToThreeKgPostPrice { get; set; }
        //public decimal NonNeighbouringTwoToThreeKgPostPrice { get; set; }

        //public decimal NeighbouringThreeToFiveKgPostPrice { get; set; }
        //public decimal NonNeighbouringThreeToFiveKgPostPrice { get; set; }

        //public decimal NeighbouringFiveToNineKgPostPrice { get; set; }
        //public decimal NonNeighbouringFiveToNineKgPostPrice { get; set; }

        //public decimal NeighbouringMoreThanNineKgPostPrice { get; set; }

        public int PostShippingMethodId { get; set; }

    }
}