using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.GoldPurchase.Models;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.GoldProductId")]
    public int GoldProductId { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.DefaultAmount")]
    public decimal DefaultAmount { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.MinimumAmount")]
    public decimal MinimumAmount { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.MaximumAmount")]
    public decimal MaximumAmount { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.PaymentSystemName")]
    public string PaymentSystemName { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.BuyCommission")]
    public decimal BuyCommission { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.SellCommission")]
    public decimal SellCommission { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Misc.GoldPurchase.Fields.IsActive")]
    public bool IsActive { get; set; }
}
