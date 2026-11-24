namespace ZarIran.Plugin.Misc.ProductManagements.Services;

public static class ProductPriceRounding
{
    public static decimal RoundUp(decimal amount)
    {
        const decimal increment = 1000m;
        return decimal.Ceiling(amount / increment) * increment;
    }
}
