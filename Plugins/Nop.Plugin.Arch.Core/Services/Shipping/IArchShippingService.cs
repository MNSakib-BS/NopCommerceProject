using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Arch.Core.Services.Shipping;
public interface IArchShippingService
{
    Task InsertUpdateShippingMethodAsync(ShippingMethod shippingMethod);
    Task<ShippingMethod> GetShippingMethodByNameAsync(string shippingMethodName);
}
