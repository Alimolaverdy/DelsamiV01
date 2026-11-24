using System;
using ZarIran.Plugin.Misc.Core.Enums;

namespace ZarIran.Plugin.Misc.Core.Helpers
{
    public class SeNameHelper
    {
        public static string GenerateSeName(SeNameType seNameType, string defaultSeName)
        {
            string prefix = seNameType switch
            {
                SeNameType.Product => "/product/",
                SeNameType.Topic => "/page/",
                SeNameType.CarBrand => "cb-",
                SeNameType.CarModel => "cm-",
                SeNameType.Category => "ct-",
                SeNameType.Manufacture => "pm-",
                _ => string.Empty
            };

            return $"{prefix}{defaultSeName}";
        }

        public static string RemoveSeNamePrefix(string seName)
        {
            if (string.IsNullOrWhiteSpace(seName))
                return seName;

            string[] prefixes =
            {
                  "cb-",
                  "cm-",
                  "ct-",
                  "pm-",
                  "product/",
                  "page/"
          };

            foreach (var prefix in prefixes)
            {
                if (seName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return seName[prefix.Length..];
            }

            return seName;
        }
    }
}
