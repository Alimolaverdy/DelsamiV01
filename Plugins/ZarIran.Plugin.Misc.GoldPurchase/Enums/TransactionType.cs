namespace ZarIran.Plugin.Misc.GoldPurchase.Enums;

/// <summary>
/// Existing values keep their numeric ids because they are stored in the database.
/// </summary>
public enum TransactionType
{
    /// <summary>Customer bought gold (grams are positive).</summary>
    Purchase = 0,

    /// <summary>Customer sold gold back (grams are negative, Amount is the Toman value owed to the customer).</summary>
    Sell = 1,

    /// <summary>Manual correction by an administrator (grams may be positive or negative).</summary>
    AdminAdjustment = 2
}
