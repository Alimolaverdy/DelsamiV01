//using Microsoft.AspNetCore.Mvc;
//using Delsami.Plugin.Payments.Digipay.Models;
//using Nop.Core;
//using Nop.Services.Localization;
//using Nop.Web.Framework.Components;
//using System.Threading.Tasks;

//namespace Delsami.Plugin.Payments.Digipay.Components
//{
//    [ViewComponent(Name = "DigiPay")]
//    public class DigiPayBPGViewComponent : NopViewComponent
//    {
//        private readonly DigiPaySettings _digiPayBPGSettings;
//        private readonly ILocalizationService _localizationService;
//        private readonly IStoreContext _storeContext;
//        private readonly IWorkContext _workContext;

//        public DigiPayBPGViewComponent(DigiPaySettings digiPayBPGSettings,
//            ILocalizationService localizationService,
//            IStoreContext storeContext,
//            IWorkContext workContext)
//        {
//            _digiPayBPGSettings = digiPayBPGSettings;
//            _localizationService = localizationService;
//            _storeContext = storeContext;
//            _workContext = workContext;
//        }

//        public async Task<IViewComponentResult> Invoke()
//        {
//            var model = new PaymentInfoModel
//            {
//                DescriptionText = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Configuration.DescriptionText")
//            };

//            return View("~/Plugins/IFC.Payments.DigiPay/Views/PaymentInfo.cshtml", model);
//        }
//    }
//}
