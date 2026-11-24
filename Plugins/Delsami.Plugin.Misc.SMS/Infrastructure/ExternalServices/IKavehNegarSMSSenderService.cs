using System;
using System.Collections.Generic;
using ZarIran.Plugin.Misc.SMS.Infrastructure.Enums;
using ZarIran.Plugin.Misc.SMS.Models.Infrastructure;

namespace ZarIran.Plugin.Misc.SMS.Infrastructure.ExternalServices
{
    public interface IKavehNegarSMSSenderService
    {
        List<SendResult> Send(string sender, List<string> receptor, string message, MessageType? type = null, DateTime? date = null, List<string> localids = null);
    }
}
