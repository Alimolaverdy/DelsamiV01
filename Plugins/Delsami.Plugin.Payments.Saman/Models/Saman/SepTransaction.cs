namespace ZarIran.Plugin.Payments.Saman.Models.Saman;

public class SepTransaction
{
    public string action { get; set; }

    public string TerminalId { get; set; }

    public long Amount { get; set; }

    public string ResNum { get; set; }

    public string RedirectUrl { get; set; }

    public string CellNumber { get; set; }
}
