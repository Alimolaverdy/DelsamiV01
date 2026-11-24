using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.AmountRestriction;

public class PaymentMethodAmountRestrictionModel
{
    public int Id { get; set; }
    public string SystemName { get; set; }
    public string FriendlyName { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class PaymentMethodAmountRestrictionList
{
    public List<PaymentMethodAmountRestrictionModel> PaymentMethods { get; set; } = new();
}