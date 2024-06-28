using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class RL_DS_OrderShippingMethodCapacityMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the Shipping Method Capacity Id
    /// </summary>
    public int? ShippingMethodCapacityId { get; set; }

    /// <summary>
    /// Gets or sets the Order Id
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the Delivery Date On Utc
    /// </summary>
    public DateTime DeliveryDateOnUtc { get; set; }
}