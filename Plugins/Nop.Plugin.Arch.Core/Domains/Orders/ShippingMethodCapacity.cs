using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class ShippingMethodCapacity : BaseEntity
{
    /// <summary>
    /// Gets or sets the AvailableDeliveryDateTimeRangeId
    /// </summary>
    public int AvailableDeliveryDateTimeRangeId { get; set; }

    /// <summary>
    /// Gets or sets the ShippingMethodId
    /// </summary>
    public int ShippingMethodId { get; set; }

    /// <summary>
    /// Gets or sets the Capacity
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Gets or sets the Deleted column
    /// </summary>
    public bool Deleted { get; set; }
}
