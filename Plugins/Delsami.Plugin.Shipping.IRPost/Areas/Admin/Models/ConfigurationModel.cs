using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.IRPost.Areas.Admin.Models
{
    public record ConfigurationModel : BaseSearchModel
    {

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.User")]
        public string User { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.ContractCode")]
        public int ContractCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.PostNodeCode")]
        public string PostNodeCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.BaseUrl")]
        public string BaseUrl { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringProvinces")]
        public IList<string> NeighbouringProvinces { get; set; } 
        
        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.EnableManualCalc")]
        public bool EnableManualCalc { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringZeroToOneKgPostPrice")]
        //public decimal NeighbouringZeroToOneKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NonNeighbouringZeroToOneKgPostPrice")]
        //public decimal NonNeighbouringZeroToOneKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringOneToTwoKgPostPrice")]
        //public decimal NeighbouringOneToTwoKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NonNeighbouringOneToTwoKgPostPrice")]
        //public decimal NonNeighbouringOneToTwoKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringTwoToThreeKgPostPrice")]
        //public decimal NeighbouringTwoToThreeKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NonNeighbouringTwoToThreeKgPostPrice")]
        //public decimal NonNeighbouringTwoToThreeKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringThreeToFiveKgPostPrice")]
        //public decimal NeighbouringThreeToFiveKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NonNeighbouringThreeToFiveKgPostPrice")]
        //public decimal NonNeighbouringThreeToFiveKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringFiveToNineKgPostPrice")]
        //public decimal NeighbouringFiveToNineKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NonNeighbouringFiveToNineKgPostPrice")]
        //public decimal NonNeighbouringFiveToNineKgPostPrice { get; set; }


        //[NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.NeighbouringMoreThanNineKgPostPrice")]
        //public decimal NeighbouringMoreThanNineKgPostPrice { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.IRPost.PostShippingMethod")]
        public int ShippingMethodId { get; set; }

        public ConfigurationModel()
        {
            AvailableNeighbouringProvinces = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();

        }
        public List<SelectListItem> AvailableNeighbouringProvinces { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }



    }
}