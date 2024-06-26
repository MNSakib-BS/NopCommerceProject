using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public interface IShippingMethodCapacityService
    {
        Task DeleteAsync(ShippingMethodCapacity shippingMethodCapacity);
        Task<IPagedList<ShippingMethodCapacity>> GetShippingMethodCapacitiesAsync(int shippingMethodId = 0, int availableDeliveryDateTimeRangeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ShippingMethodCapacity> GetShippingMethodCapacityAsync(int id);
        Task SaveAsync(ShippingMethodCapacity shippingMethodCapacity);
        Task UpdateAsync(ShippingMethodCapacity shippingMethodCapacity);
    }
}