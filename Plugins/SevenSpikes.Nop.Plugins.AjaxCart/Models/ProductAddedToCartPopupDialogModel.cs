using Nop.Web.Models.ShoppingCart;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Models
{
  public class ProductAddedToCartPopupDialogModel
  {
    public ProductAddedToCartPopupDialogModel()
    {
      this.MiniShoppingCart = new MiniShoppingCartModel();
      this.EnableRelatedProductsInPopup = false;
      this.EnableCrossSellProductsInPopup = false;
    }

    public MiniShoppingCartModel MiniShoppingCart { get; set; }

    public bool EnableRelatedProductsInPopup { get; set; }

    public bool EnableCrossSellProductsInPopup { get; set; }
  }
}
