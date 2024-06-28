using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class GenerateOTPEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public GenerateOTPEvent(Order order)
    {
        Order = order;
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}
