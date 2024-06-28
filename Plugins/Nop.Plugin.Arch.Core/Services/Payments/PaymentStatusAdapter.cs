using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Arch.Core.Domains.Payments;

namespace Nop.Plugin.Arch.Core.Services.Payments;
public static class PaymentStatusAdapter
{
    public static CustomPaymentStatus ToCustomPaymentStatus(PaymentStatus paymentStatus)
    {
        return paymentStatus switch
        {
            PaymentStatus.Pending => CustomPaymentStatus.Pending,
            PaymentStatus.Authorized => CustomPaymentStatus.Authorized,
            PaymentStatus.Paid => CustomPaymentStatus.Paid,
            PaymentStatus.PartiallyRefunded => CustomPaymentStatus.PartiallyRefunded,
            PaymentStatus.Refunded => CustomPaymentStatus.Refunded,
            PaymentStatus.Voided => CustomPaymentStatus.Voided,
            
            _ => throw new ArgumentOutOfRangeException(nameof(paymentStatus), paymentStatus, null)
        };
    }

    public static PaymentStatus ToPaymentStatus(CustomPaymentStatus customPaymentStatus)
    {
        return customPaymentStatus switch
        {
            CustomPaymentStatus.Pending => PaymentStatus.Pending,
            CustomPaymentStatus.Authorized => PaymentStatus.Authorized,
            CustomPaymentStatus.Paid => PaymentStatus.Paid,
            CustomPaymentStatus.PartiallyRefunded => PaymentStatus.PartiallyRefunded,
            CustomPaymentStatus.Refunded => PaymentStatus.Refunded,
            CustomPaymentStatus.Voided => PaymentStatus.Voided,
            CustomPaymentStatus.Failed => (PaymentStatus)CustomPaymentStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(customPaymentStatus), customPaymentStatus, null)
        };
    }
}
