using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public interface IAvailableDeliveryDateTimeRangeService
    {
        Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int id);
        Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int dayOfWeek, int availableDeliveryTimeRangeId);
        Task SaveAsync(AvailableDeliveryDateTimeRange entity);
        Task UpdateAsync(AvailableDeliveryDateTimeRange entity);
    }
}
