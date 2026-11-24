using Nop.Web.Models.ShoppingCart;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Models
{
  public class ProductAddedToWishlistPopupDialogModel
  {
    public ProductAddedToWishlistPopupDialogModel()
    {
      this.WishlistShoppingCartItemModel = new WishlistModel.ShoppingCartItemModel();
      this.EnableRelatedProductsInPopup = false;
      this.EnableCrossSellProductsInPopup = false;
    }

    public WishlistModel.ShoppingCartItemModel WishlistShoppingCartItemModel { get; set; }

    public bool EnableRelatedProductsInPopup { get; set; }

    public bool EnableCrossSellProductsInPopup { get; set; }
  }
}
