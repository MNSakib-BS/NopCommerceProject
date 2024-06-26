using LinqToDB;
using Nop.Core.Domain.Orders;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure
{
    public interface IHealthCheckService
    {
        Task<Models.OrderCheckResult> CheckOrderHealth();
    }

    public class HealthCheckService :IHealthCheckService
    {
        protected readonly IRepository<Order> _orderRepository;
        public HealthCheckService(IRepository<Order> orderRepository)
        {
                _orderRepository = orderRepository;
        }

        public async Task<Models.OrderCheckResult> CheckOrderHealth()
        {
            var checkOrderDate  = DateTime.Now.AddMinutes(-5);
            var query = from order in _orderRepository.Table.Where(p => p.OrderStatusId != 40 && p.PaymentStatusId == 30  && p.TransactionTrackingNumber == 0 && p.CreatedOnUtc <= checkOrderDate)
                        select new
                        {
                            order.Id
                        };
            var hasUnAcceptedOrders = await query.ToListAsync();

            var result = new Models.OrderCheckResult
            {
                HasUnacceptedOrders = hasUnAcceptedOrders.Any()
            };

            return result;
        }
    }
}
