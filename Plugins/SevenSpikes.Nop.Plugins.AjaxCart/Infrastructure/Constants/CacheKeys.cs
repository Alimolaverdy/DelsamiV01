using Nop.Core.Caching;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Infrastructure.Constants
{
  public static class CacheKeys
  {
    public const string AjaxCartBaseCachePattern = "nop.pres.7spikes.ajaxcart";
    public const string EncryptionKey = "SS_AjaxCart_EK_1";

    public static CacheKey AjaxCartButtonsModelCacheKey => new CacheKey("nop.pres.7spikes.ajaxcart{0}-{1}-{2}-{3}", new[] { "nop.pres.7spikes.ajaxcart" });
  }
}
