namespace Delsami.Plugin.Payments.Saman.Models.Saman;

public class VerifyTransactionRequest
{
    public string RefNum { get; set; }

    public long TerminalNumber { get; set; }
}
