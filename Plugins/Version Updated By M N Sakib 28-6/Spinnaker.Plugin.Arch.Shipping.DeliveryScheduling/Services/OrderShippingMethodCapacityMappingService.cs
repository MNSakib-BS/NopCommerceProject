using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public class OrderShippingMethodCapacityMappingService : IOrderShippingMethodCapacityMappingService
    {
        private readonly IRepository<OrderShippingMethodCapacityMapping> _shippingMethodCapacityRepository;
        private readonly IEventPublisher _eventPublisher;

        public OrderShippingMethodCapacityMappingService(IRepository<OrderShippingMethodCapacityMapping> shippingMethodCapacityRepository, IEventPublisher eventPublisher)
        {
            _shippingMethodCapacityRepository = shippingMethodCapacityRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<int> GetPlacedOrdersCountAsync(DateTime date, int shippingMethodCapacityId = 0)
        {
            var query = _shippingMethodCapacityRepository.Table;

            query = query.Where(i => i.DeliveryDateOnUtc == date);

            if (shippingMethodCapacityId > 1)
            {
                query = query.Where(i => i.ShippingMethodCapacityId == shippingMethodCapacityId);
            }

            return await query.CountAsync();
        }

        public async Task<Dictionary<Tuple<DateTime, int>, int>> GetPlacedOrdersCountsForAllAsync(DateTime startDate, int daysRange, int minShippingMethodCapacityId = 0)
        {
            startDate = startDate.Date;
            var endDate = startDate.AddDays(daysRange);

            var query = from record in _shippingMethodCapacityRepository.Table
                        where record.DeliveryDateOnUtc >= startDate && record.DeliveryDateOnUtc < endDate
                              && (minShippingMethodCapacityId == 0 || record.ShippingMethodCapacityId > minShippingMethodCapacityId)
                        group record by new { record.DeliveryDateOnUtc, record.ShippingMethodCapacityId } into g
                        select new
                        {
                            Date = g.Key.DeliveryDateOnUtc,
                            CapacityId = g.Key.ShippingMethodCapacityId,
                            Count = g.Count()
                        };

            var results = await query.ToListAsync();
            var countsDictionary = new Dictionary<Tuple<DateTime, int>, int>();

            foreach (var result in results)
            {
                var key = Tuple.Create(result.Date, result.CapacityId);
                countsDictionary[key] = result.Count;
            }

            return countsDictionary;
        }

        public async Task<OrderShippingMethodCapacityMapping> GetOrderShippingMethodCapacityMappingByOrderIdAsync(int orderId)
        {
            return await _shippingMethodCapacityRepository.Table.FirstOrDefaultAsync(w => w.OrderId == orderId);
        }

        public async Task InsertAsync(OrderShippingMethodCapacityMapping orderShippingMethodCapacityMapping)
        {
            if (orderShippingMethodCapacityMapping == null)
                throw new ArgumentNullException(nameof(orderShippingMethodCapacityMapping));

            await _shippingMethodCapacityRepository.InsertAsync(orderShippingMethodCapacityMapping);

            EventPublisherExtensions.EntityInserted<OrderShippingMethodCapacityMapping>(_eventPublisher, orderShippingMethodCapacityMapping);
        }
    }
}
