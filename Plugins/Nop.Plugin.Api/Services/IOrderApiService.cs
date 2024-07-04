using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderApiService
    {
        Task<IList<Order>> GetOrdersByCustomerIdAsync(int customerId);

        Task<IList<Order>> GetOrdersAsync(
            IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            int sinceId = Constants.Configurations.DefaultSinceId, OrderStatus? status = null, PaymentStatus? paymentStatus = null,
            ShippingStatus? shippingStatus = null, int? customerId = null, int? storeId = null);

        Task<Order> GetOrderByIdAsync(int orderId);

        Task<int> GetOrdersCountAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
            int? customerId = null, int? storeId = null);

        Task<IList<Order>> GetOrdersForStoresAsync(IList<int> storeIds, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            ICollection<int> statusIds = null, ICollection<int> paymentStatusIds = null,
            ICollection<int> shippingStatusIds = null, int? allocatedToDriverId = null, DateTime? maxEstimatedDeliveryDate = null);
    }
}
