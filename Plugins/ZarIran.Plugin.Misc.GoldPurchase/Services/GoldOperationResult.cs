using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public class GoldOperationResult
{
    public bool Success { get; private set; }
    public string Error { get; private set; }
    public GoldTransaction Transaction { get; private set; }

    public static GoldOperationResult Ok(GoldTransaction transaction)
        => new() { Success = true, Transaction = transaction };

    public static GoldOperationResult Fail(string error)
        => new() { Success = false, Error = error };
}
