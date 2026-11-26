using Delsami.Plugin.Shipping.IRPost.Models;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services;

public interface IIRPostService
{
    Task<IList<ShippingOption>> PrepareShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest);

    Task<(bool IsCallSuccess, GetBarCodeResponse Response)> GetBarCodeAsync(GetBarCodeRequest getPriceRequest);
}
