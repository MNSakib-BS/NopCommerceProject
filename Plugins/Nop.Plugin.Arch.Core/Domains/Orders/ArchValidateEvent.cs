using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Arch validate event
/// </summary>
public class ArchValidateEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public ArchValidateEvent(Order order, bool orderCompleted = false)
    {
        Order = order;
        OrderCompleted = orderCompleted;
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }

    /// <summary>
    /// bool check if the order has been completed
    /// </summary>
    public bool OrderCompleted { get; }
}