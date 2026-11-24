using System;
using System.Linq;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public static class GoldSheba
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 100)
            return null;

        var normalized = new string(value.Where(c => !char.IsWhiteSpace(c) && c != '-')
            .Select(c => c switch
            {
                >= '۰' and <= '۹' => (char)('0' + c - '۰'),
                >= '٠' and <= '٩' => (char)('0' + c - '٠'),
                _ => char.ToUpperInvariant(c)
            }).ToArray());

        if (normalized.StartsWith("IR", StringComparison.Ordinal))
            normalized = normalized[2..];
        if (normalized.Length != 24 || normalized.Any(c => c < '0' || c > '9'))
            return null;

        var remainder = 0;
        foreach (var digit in normalized[2..] + "1827" + normalized[..2])
            remainder = (10 * remainder + digit - '0') % 97;

        return remainder == 1 ? "IR" + normalized : null;
    }
}
