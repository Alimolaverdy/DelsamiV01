using Microsoft.AspNetCore.Mvc;
using static Nop.Web.Models.Catalog.CategoryModel;

namespace SevenSpikes.Nop.Plugins.JCarousel.Components;

[ViewComponent(Name = "JCarouselCategoryBox")]
public class JCarouselCategoryBoxComponent : ViewComponent
{
    public IViewComponentResult Invoke(SubCategoryModel model)
    {
        return View<SubCategoryModel>("CategoryBox", model);
    }
}
