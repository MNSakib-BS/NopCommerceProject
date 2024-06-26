using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Data;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;



namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public class AvailableDeliveryDateTimeRangeService : IAvailableDeliveryDateTimeRangeService
    {
        private readonly IRepository<AvailableDeliveryDateTimeRange> _availableDeliveryDateTimeRangeRepo;
        private readonly IRepository<AvailableDeliveryTimeRange> _availableDeliveryTimeRangeRepo;
        private readonly INopDataProvider _dataProvider;

        public AvailableDeliveryDateTimeRangeService(
            IRepository<AvailableDeliveryDateTimeRange> availableDeliveryDateTimeRangeRepo,
            IRepository<AvailableDeliveryTimeRange> availableDeliveryTimeRangeRepo,
            INopDataProvider dataProvider)
        {
            _availableDeliveryDateTimeRangeRepo = availableDeliveryDateTimeRangeRepo;
            _availableDeliveryTimeRangeRepo = availableDeliveryTimeRangeRepo;
            _dataProvider = dataProvider;
        }

        public async Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int id)
        {
            if (id == 0)
                return null;

            return await _availableDeliveryDateTimeRangeRepo.GetByIdAsync(id);
        }

        public async Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int dayOfWeek, int availableDeliveryTimeRangeId)
        {
            return await _availableDeliveryDateTimeRangeRepo.Table
                .Where(w => w.DayOfWeek == dayOfWeek && w.AvailableDeliveryTimeRangeId == availableDeliveryTimeRangeId)
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(AvailableDeliveryDateTimeRange entity)
        {
            await _availableDeliveryDateTimeRangeRepo.InsertAsync(entity);
         
        }

        public async Task UpdateAsync(AvailableDeliveryDateTimeRange entity)
        {
            await _availableDeliveryDateTimeRangeRepo.UpdateAsync(entity);
            
        }
    }
}
