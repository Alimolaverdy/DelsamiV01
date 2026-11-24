namespace SevenSpikes.Nop.Plugins.AjaxCart.Models
{
  public class AddToCartResult
  {
    public bool Success { get; set; }

    public int ProductId { get; set; }

    public string Message { get; set; }

    public string SuccessMessage { get; set; }
  }
}
