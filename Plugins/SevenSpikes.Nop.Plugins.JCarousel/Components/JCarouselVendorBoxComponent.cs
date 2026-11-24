using Microsoft.AspNetCore.Mvc;
using Nop.Web.Models.Catalog;

namespace SevenSpikes.Nop.Plugins.JCarousel.Components;

[ViewComponent(Name = "JCarouselVendorBox")]
public class JCarouselVendorBoxComponent : ViewComponent
{
    public IViewComponentResult Invoke(VendorModel model)
    {
        return View<VendorModel>("VendorBox", model);
    }
}
