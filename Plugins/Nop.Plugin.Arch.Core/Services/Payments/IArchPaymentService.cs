using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Arch.Core.Domains.Payments;

namespace Nop.Plugin.Arch.Core.Services.Payments;
public interface IArchPaymentService
{
    Task<Dictionary<string, dynamic>> EftPayRequestAsync(PaymentInfoModel paymentInfoModel);
    Task<PaymentStatus> GetOrderPaymentStatusAsync(string checkoutId);
    Task ArchRefundAsync(Order order, string description, int archRefundType, decimal refundAmount);
}
