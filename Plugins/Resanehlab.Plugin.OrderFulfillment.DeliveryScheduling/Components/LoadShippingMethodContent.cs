using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.UI;
using System.Threading.Tasks;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Components
{
    public class LoadShippingMethodContent : NopViewComponent
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly OrderSettings _orderSettings;
        private readonly INopHtmlHelper _nopHtmlHelper;

        public LoadShippingMethodContent(DeliverySchedulingSetting deliverySchedulingSetting,
            OrderSettings orderSettings,
            INopHtmlHelper nopHtmlHelper)
        {
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _orderSettings = orderSettings;
            _nopHtmlHelper = nopHtmlHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!_deliverySchedulingSetting.EnablePlugin)
                return Content(string.Empty);

            var model = new ShippingMethodContentModel();

            model.IsOpc = _orderSettings.OnePageCheckoutEnabled;

            _nopHtmlHelper.AddCssFileParts($"{Constants.PluginOutputPathWithoutTilde}{Constants.DeliverySchedulingCssPath}style.css");

            return View("~/Plugins/ZarIran.OrderFulfillment.DeliveryScheduling/Views/Shared/Components/LoadShippingMethodContent/Default.cshtml", model);
        }
    }
}
