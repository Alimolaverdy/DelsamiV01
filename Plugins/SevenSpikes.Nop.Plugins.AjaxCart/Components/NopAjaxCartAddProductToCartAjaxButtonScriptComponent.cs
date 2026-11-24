using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Components
{
    public class NopAjaxCartAddProductToCartAjaxButtonScriptComponent :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/SevenSpikes.Nop.Plugins.AjaxCart/Views/Components/NopAjaxCartAddProductToCartAjaxButtonScript/Default.cshtml");
        }
    }
}
