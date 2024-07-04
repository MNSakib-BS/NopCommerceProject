using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Infrastructure;
using Nop.Services.Configuration;
using Nop.Core.Domain.Mobile;
using Nop.Services.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;

namespace Nop.Plugin.Api.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IResanehlabService _resanehlabService;

        public OrderApiService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;

            _settingService = EngineContext.Current.Resolve<ISettingService>();
            _shippingService = EngineContext.Current.Resolve<IShippingService>();
            _resanehlabService = EngineContext.Current.Resolve<IResanehlabService>();
        }

        public async Task<IList<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var query = from order in _orderRepository.Table
                        where order.CustomerId == customerId && !order.Deleted
                        orderby order.Id
                        select order;

            return new ApiList<Order>(query, 0, Constants.Configurations.MaxLimit);
        }

        public async Task<IList<Order>> GetOrdersAsync(
            IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            int sinceId = Constants.Configurations.DefaultSinceId,
            OrderStatus? status = null, PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null,
            int? storeId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, ids, customerId, storeId);

            if (sinceId > 0)
            {
                query = query.Where(order => order.Id > sinceId);
            }

            return new ApiList<Order>(query, page - 1, limit);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return null;
            }

            return _orderRepository.Table.FirstOrDefault(order => order.Id == orderId && !order.Deleted);
        }

        public async Task<int> GetOrdersCountAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
            int? customerId = null, int? storeId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, customerId: customerId, storeId: storeId);

            return query.Count();
        }

        private IQueryable<Order> GetOrdersQuery(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, IList<int> ids = null,
            int? customerId = null, int? storeId = null, int? allocatedToDriverId = null, DateTime? maxEstimatedDeliveryDate = null)
        {
            var query = _orderRepository.Table;

            if (customerId != null)
            {
                query = query.Where(order => order.CustomerId == customerId);
            }

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (status != null)
            {
                query = query.Where(order => order.OrderStatusId == (int)status);
            }

            if (paymentStatus != null)
            {
                query = query.Where(order => order.PaymentStatusId == (int)paymentStatus);
            }

            if (shippingStatus != null)
            {
                query = query.Where(order => order.ShippingStatusId == (int)shippingStatus);
            }

            query = query.Where(order => !order.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(order => order.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());
            }

            if (createdAtMax != null)
            {
                query = query.Where(order => order.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());
            }

            if (storeId != null)
            {
                query = query.Where(order => order.StoreId == storeId);
            }

            var mobileSettings = _settingService?.LoadSetting<MobileSettings>(storeId ?? 0);
            if (mobileSettings != null)
            {
                var selectedShippingMethodIds = mobileSettings.JoinedSelectedShippingMethodIds
                    ?.Split(',')
                    ?.Where(i => !string.IsNullOrWhiteSpace(i))
                    ?.Select(i => Convert.ToInt32(i))
                    ?.ToList() ?? new List<int>();

                var shippingMethods = _shippingService.GetAllShippingMethods().Where(i => selectedShippingMethodIds.Contains(i.Id)).Select(i => i.Name).ToArray();

                if (shippingMethods.Length > 0)
                {
                    query = query.Where(i => shippingMethods.Contains(i.ShippingMethod));
                }
            }            

            query = query.Where(order => order.AllocatedToDriverId == allocatedToDriverId);

            if (maxEstimatedDeliveryDate.HasValue && _resanehlabService != null)
            {
                var currentQuery = query.ToList();
                query = currentQuery.Where(i => IsOrderEstimatedDeliveryDateLessThanEqualTo(i.Id, maxEstimatedDeliveryDate.Value)).AsQueryable();
            }

            query = query.OrderBy(order => order.Id);

            return query;
        }

        private async Task<IQueryable<Order>> GetOrdersQueryAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, ICollection<int> statusIds = null, ICollection<int> paymentStatusIds = null,
            ICollection<int> shippingStatusIds = null, IList<int> ids = null, int? customerId = null, int? storeId = null, 
            int? allocatedToDriverId = null, DateTime? maxEstimatedDeliveryDate = null)
        {
            var query = _orderRepository.Table;

            if (customerId != null)
            {
                query = query.Where(order => order.CustomerId == customerId);
            }

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (statusIds != null)
            {
                query = query.Where(order => statusIds.Contains(order.OrderStatusId));
            }

            if (paymentStatusIds != null)
            {
                query = query.Where(order => paymentStatusIds.Contains(order.PaymentStatusId));
            }

            if (shippingStatusIds != null)
            {
                query = query.Where(order => shippingStatusIds.Contains(order.ShippingStatusId));
            }

            query = query.Where(order => !order.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(order => order.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());
            }

            if (createdAtMax != null)
            {
                query = query.Where(order => order.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());
            }

            if (storeId != null)
            {
                query = query.Where(order => order.StoreId == storeId);
            }

            var mobileSettings = _settingService?.LoadSetting<MobileSettings>(storeId ?? 0);
            if (mobileSettings != null)
            {
                var selectedShippingMethodIds = mobileSettings.JoinedSelectedShippingMethodIds
                    ?.Split(',')
                    ?.Where(i => !string.IsNullOrWhiteSpace(i))
                    ?.Select(i => Convert.ToInt32(i))
                    ?.ToList() ?? new List<int>();

                var shippingMethods = (await _shippingService.GetAllShippingMethodsAsync()).Where(i => selectedShippingMethodIds.Contains(i.Id)).Select(i => i.Name).ToArray();

                if (shippingMethods.Length > 0)
                {
                    query = query.Where(i => shippingMethods.Contains(i.ShippingMethod));
                }
            }

            query = query.Where(order => order.AllocatedToDriverId == allocatedToDriverId);

            if (maxEstimatedDeliveryDate.HasValue && _resanehlabService != null)
            {
                var currentQuery = query.ToList();
                query = currentQuery.Where(i => IsOrderEstimatedDeliveryDateLessThanEqualTo(i.Id, maxEstimatedDeliveryDate.Value)).AsQueryable();
            }

            query = query.OrderBy(order => order.Id);

            return query;
        }

        public async Task< IList<Order>> GetOrdersForStoresAsync(IList<int> storeIds, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            ICollection<int> statusIds = null, ICollection<int> paymentStatusIds = null,
            ICollection<int> shippingStatusIds = null, int? allocatedToDriverId = null, DateTime? maxEstimatedDeliveryDate = null)
        {
            var query =await GetOrdersQueryAsync(createdAtMin, createdAtMax, statusIds, paymentStatusIds, shippingStatusIds,
                allocatedToDriverId: allocatedToDriverId, maxEstimatedDeliveryDate: maxEstimatedDeliveryDate);

            query = query.Where(i => storeIds.Contains(i.StoreId));

            return new ApiList<Order>(query, page - 1, limit);
        }

        private bool IsOrderEstimatedDeliveryDateLessThanEqualTo(int orderId, DateTime dateTime)
        {
            var estimatedDeliveryDate = _resanehlabService.GetEstimatedDateForOrder(orderId);
            return estimatedDeliveryDate.HasValue && estimatedDeliveryDate <= dateTime;
        }
    }
}
