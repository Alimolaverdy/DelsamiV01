using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ZarIran.Plugin.Misc.Core.Components
{
    public class ToEnglishNumberViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ZarIran.Misc.Core/Views/Shared/Components/ToEnglishNumber/Default.cshtml");
        }
    }
}
