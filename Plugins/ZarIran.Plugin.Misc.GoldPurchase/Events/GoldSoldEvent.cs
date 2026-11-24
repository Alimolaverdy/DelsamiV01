using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Events;

/// <summary>
/// Published after a customer sold gold. The gold has already been debited and the ledger row written.
/// Published after the gold sale and monetary wallet credit are committed.
/// Transaction.Amount is the Toman value owed to the customer.
/// </summary>
public class GoldSoldEvent
{
    public GoldSoldEvent(GoldTransaction transaction)
    {
        Transaction = transaction;
    }

    public GoldTransaction Transaction { get; }
}
