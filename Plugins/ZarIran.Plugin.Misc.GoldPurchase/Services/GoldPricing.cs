using System;
using System.Globalization;
using System.Linq;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

/// <summary>Single place for the price/rounding rules so the page preview and the server always agree.</summary>
public static class GoldPricing
{
    public const int GramDecimals = 8;

    public static decimal RoundGrams(decimal grams)
        => Math.Round(grams, GramDecimals, MidpointRounding.ToZero);

    /// <summary>Purchase quote includes the configured percentage; kept precise until the final amount is rounded.</summary>
    public static decimal GetBuyPricePerGram(decimal marketPricePerGram, decimal buyCommissionPercent)
    {
        var commission = Math.Min(Math.Max(buyCommissionPercent, 0m), 100m);
        return marketPricePerGram * (100m + commission) / 100m;
    }

    /// <summary>
    /// Price the customer receives per gram when selling: market price minus the sell commission (percent).
    /// The final sale amount is rounded down to a whole Toman.
    /// </summary>
    public static decimal GetSellPricePerGram(decimal marketPricePerGram, decimal sellCommissionPercent)
    {
        var commission = Math.Min(Math.Max(sellCommissionPercent, 0m), 100m);
        return marketPricePerGram * (100m - commission) / 100m;
    }

    /// <summary>Round the final purchase amount, not the per-gram quote.</summary>
    public static decimal GetBuyAmount(decimal grams, decimal buyPricePerGram)
        => Math.Ceiling(grams * buyPricePerGram);

    public static bool TryParseTomanAmount(string input, out decimal amount)
    {
        amount = 0m;
        if (string.IsNullOrWhiteSpace(input) || input.Length > 64)
            return false;

        var digits = new string(input.Where(c => !char.IsWhiteSpace(c) && c != ',' && c != '٬')
            .Select(c => c switch
            {
                >= '۰' and <= '۹' => (char)('0' + c - '۰'),
                >= '٠' and <= '٩' => (char)('0' + c - '٠'),
                _ => c
            }).ToArray());
        return decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out amount);
    }

    /// <summary>Turn a Toman budget into grams without exceeding the entered budget.</summary>
    public static decimal GetBuyWeight(decimal amount, decimal pricePerGram)
        => amount > 0 && pricePerGram > 0 ? RoundGrams(amount / pricePerGram) : 0m;

    /// <summary>The smallest supported weight whose net sale value reaches the requested amount.</summary>
    public static decimal GetSellWeight(decimal amount, decimal pricePerGram)
        => amount > 0 && pricePerGram > 0 ? Math.Ceiling(amount / pricePerGram * 100000000m) / 100000000m : 0m;

    /// <summary>Toman value of a sale, rounded down to a whole Toman.</summary>
    public static decimal GetSellAmount(decimal grams, decimal sellPricePerGram)
        => Math.Floor(grams * sellPricePerGram);
}
