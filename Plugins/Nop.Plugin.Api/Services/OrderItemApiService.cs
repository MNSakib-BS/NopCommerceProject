using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DataStructures;
using Nop.Services.Catalog;
using Nop.Services.Orders;

namespace Nop.Plugin.Api.Services
{
    public class OrderItemApiService : IOrderItemApiService
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderItemApiService(IOrderService orderService
                                  ,IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }
        public async Task<IList<OrderItem>> GetOrderItemsForOrderAsync(Order order, int limit, int page, int sinceId)
        {
            var orderItems =(await _orderService.GetOrderItemsAsync(order.Id)).AsQueryable();

            return new ApiList<OrderItem>(orderItems, page - 1, limit);
        }

        public async Task<int> GetOrderItemsCountAsync(Order order)
        {
            var orderItemsCount =(await _orderService.GetOrderItemsAsync(order.Id)).Count();

            return orderItemsCount;
        }

    }
}
