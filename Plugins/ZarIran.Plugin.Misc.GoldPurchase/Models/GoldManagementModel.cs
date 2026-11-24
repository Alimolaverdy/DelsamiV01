using System.Collections.Generic;
using Nop.Web.Framework.Models;
namespace ZarIran.Plugin.Misc.GoldPurchase.Models;
public record GoldManagementModel : BaseNopModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public IList<CustomerRow> Customers { get; set; } = new List<CustomerRow>();
    public record CustomerRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
    }
}
