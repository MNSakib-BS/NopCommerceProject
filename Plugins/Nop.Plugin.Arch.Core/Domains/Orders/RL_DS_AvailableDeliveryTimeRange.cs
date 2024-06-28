using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class RL_DS_AvailableDeliveryTimeRange : BaseEntity
{
    /// <summary>
    /// Gets or sets the Time variable
    /// </summary>
    public string? Time { get; set; }

    /// <summary>
    /// Gets or sets the Store Id
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the Display Order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the Deleted value
    /// </summary>
    public bool Deleted { get; set; }
}
