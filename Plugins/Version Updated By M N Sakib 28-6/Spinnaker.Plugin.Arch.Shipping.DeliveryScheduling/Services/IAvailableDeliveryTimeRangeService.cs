using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public interface IAvailableDeliveryTimeRangeService
    {
        Task<AvailableDeliveryTimeRange> GetAvailableDeliveryTimeRangeAsync(int id);
    }
}
