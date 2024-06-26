using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using System;
using System.Collections.Generic;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{

    public interface IOrderShippingMethodCapacityMappingService
    {
        Task<OrderShippingMethodCapacityMapping> GetOrderShippingMethodCapacityMappingByOrderIdAsync(int orderId);
        Task<int> GetPlacedOrdersCountAsync(DateTime date, int shippingMethodCapacityId = 0);
        Task<Dictionary<Tuple<DateTime, int>, int>> GetPlacedOrdersCountsForAllAsync(DateTime startDate, int daysRange, int minShippingMethodCapacityId = 0);
        Task InsertAsync(OrderShippingMethodCapacityMapping orderShippingMethodCapacityMapping);
    }
}