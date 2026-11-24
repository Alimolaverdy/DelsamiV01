using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers
{
    public record CustomerShebaNumberListModel : BaseNopModel
    {
        public CustomerShebaNumberListModel()
        {
            CustomerShebaNumbers = new List<CustomerShebaNumberModel>();
        }

        public List<CustomerShebaNumberModel> CustomerShebaNumbers { get; set; }
    }
}