using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Data;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public class AvailableDeliveryTimeRangeService : IAvailableDeliveryTimeRangeService
    {
        private readonly IRepository<AvailableDeliveryTimeRange> _availableDeliveryTimeRangeRepo;

        public AvailableDeliveryTimeRangeService(IRepository<AvailableDeliveryTimeRange> availableDeliveryTimeRangeRepo)
        {
            _availableDeliveryTimeRangeRepo = availableDeliveryTimeRangeRepo;
        }

        public async Task<AvailableDeliveryTimeRange> GetAvailableDeliveryTimeRangeAsync(int id)
        {
            if (id == 0)
                return null;

            return await _availableDeliveryTimeRangeRepo.GetByIdAsync(id);
        }
    }
}
