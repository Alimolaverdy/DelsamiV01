using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ZarIran.Plugin.Misc.GoldPurchase.Models;

public class GoldPayoutModel
{
    public decimal Available { get; init; }
    public Guid RequestKey { get; init; } = Guid.NewGuid();
    public IList<SelectListItem> Shebas { get; init; } = new List<SelectListItem>();
    public string Message { get; init; }
    public string Error { get; init; }
}
