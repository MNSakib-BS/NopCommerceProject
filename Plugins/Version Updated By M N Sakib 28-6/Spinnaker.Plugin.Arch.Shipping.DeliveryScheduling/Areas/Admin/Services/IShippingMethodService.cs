using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services
{
    public interface IShippingMethodService
    {
        Task DeleteShippingMethodCapacityByDeliveryDateIdAsync(int availableDeliveryDateTimeRangeId);
        Task<IQueryable<ShippingMethod>> GetAllAsync(int id = 0, string name = "");
        Task<IQueryable<ShippingMethodCapacity>> GetAllShippingMethodsAsync(int availableDeliveryDateTimeRangeId = 0, int shippingMethodId = 0, bool deleted = false);
        Task<ShippingMethodCapacity> GetShippingCapacityByIdAsync(int id);
        Task<IQueryable<ShippingMethodCapacity>> GetShippingMethodCapacityByShippingMethodIdAsync(int shippingMethodId, bool deleted = false);
        Task InsertShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity);
        Task UpdateShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity);
    }
}