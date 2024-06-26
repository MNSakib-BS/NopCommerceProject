using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Linq;
using Nop.Core.Events;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services
{
    public class TimeDeliveryService : ITimeDeliveryService
    {

        private readonly IRepository<AvailableDeliveryTimeRange> _availableDeliveryTimeRangeRepository;
        private readonly IRepository<AvailableDeliveryDateTimeRange> _availableDeliveryDateTimeRange;
        private readonly IRepository<ShippingMethodCapacity> _shippingMethodCapacity;
        private readonly IEventPublisher _eventPublisher;

        public TimeDeliveryService(IRepository<AvailableDeliveryTimeRange> availableDeliveryTimeRangeRepository, IEventPublisher eventPublisher, IRepository<AvailableDeliveryDateTimeRange> availableDeliveryDateTimeRange, IRepository<ShippingMethodCapacity> shippingMethodCapacity)
        {
            _availableDeliveryTimeRangeRepository = availableDeliveryTimeRangeRepository;
            _eventPublisher = eventPublisher;
            _availableDeliveryDateTimeRange = availableDeliveryDateTimeRange;
            _shippingMethodCapacity = shippingMethodCapacity;
        }

        public async Task<IPagedList<AvailableDeliveryTimeRange>> GetAllDeliveryTimeRangeAsync(int storeId = 0, int id = 0, bool deleted = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var timeRangeQuery = _availableDeliveryTimeRangeRepository.Table;

            timeRangeQuery = timeRangeQuery.Where(i => i.Deleted == deleted);

            if (storeId > 0)
                timeRangeQuery = timeRangeQuery.Where(i => i.StoreId == storeId);

            if (id > 0)
                timeRangeQuery = timeRangeQuery.Where(i => i.Id == id);

            return await Task.FromResult(new PagedList<AvailableDeliveryTimeRange>((IList<AvailableDeliveryTimeRange>)timeRangeQuery, pageIndex, pageSize));
        }

        public async Task<AvailableDeliveryTimeRange> GetByIdAsync(int id)
        {
            var timeRangeQuery = _availableDeliveryTimeRangeRepository.Table;
            var timeRange = await timeRangeQuery.FirstOrDefaultAsync(i => i.Id == id && !i.Deleted);
            return timeRange;
        }

        public async Task<IQueryable<AvailableDeliveryDateTimeRange>> GetAllDeliveryDateTimeRangeAsync(int availableDeliveryTimeRangeId = 0, int id = 0)
        {
            var dateTimeRange = _availableDeliveryDateTimeRange.Table;

            if (availableDeliveryTimeRangeId > 0)
                dateTimeRange = dateTimeRange.Where(i => i.AvailableDeliveryTimeRangeId == availableDeliveryTimeRangeId);

            if (id > 0)
                dateTimeRange = dateTimeRange.Where(i => i.Id == id);

            return await Task.FromResult(dateTimeRange);
        }

        public async Task UpdateTimeRangeAsync(AvailableDeliveryTimeRange timeRange)
        {
            if (timeRange == null)
                throw new ArgumentNullException(nameof(timeRange));

            await _availableDeliveryTimeRangeRepository.UpdateAsync(timeRange);
            await _eventPublisher.EntityUpdatedAsync(timeRange);
        }

        public async Task InsertTimeRangeAsync(AvailableDeliveryTimeRange timeRange)
        {
            if (timeRange == null)
                throw new ArgumentNullException(nameof(timeRange));

            await _availableDeliveryTimeRangeRepository.InsertAsync(timeRange);
            await _eventPublisher.EntityInsertedAsync(timeRange);
        }

        public async Task InsertDateTimeRangeAsync(AvailableDeliveryDateTimeRange dateTimeRange)
        {
            if (dateTimeRange == null)
                throw new ArgumentNullException(nameof(dateTimeRange));

            await _availableDeliveryDateTimeRange.InsertAsync(dateTimeRange);
            await _eventPublisher.EntityInsertedAsync(dateTimeRange);
        }

        public async Task DeleteTimeRangeAsync(AvailableDeliveryTimeRange timeRange)
        {
            if (timeRange == null)
                throw new ArgumentNullException(nameof(timeRange));

            timeRange.Deleted = true;
            await _availableDeliveryTimeRangeRepository.UpdateAsync(timeRange);
            await _eventPublisher.EntityDeletedAsync(timeRange);
        }
    }
}
