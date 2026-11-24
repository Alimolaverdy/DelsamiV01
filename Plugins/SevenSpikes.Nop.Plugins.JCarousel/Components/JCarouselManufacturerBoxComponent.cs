using Microsoft.AspNetCore.Mvc;
using Nop.Web.Models.Catalog;

namespace SevenSpikes.Nop.Plugins.JCarousel.Components;

[ViewComponent(Name = "JCarouselManufacturerBox")]
public class JCarouselManufacturerBoxComponent : ViewComponent
{
    public IViewComponentResult Invoke(ManufacturerModel model)
    {
        return View<ManufacturerModel>("ManufacturerBox", model);
    }
}
