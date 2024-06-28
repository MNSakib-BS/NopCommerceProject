using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class AvailableDeliveryDateTimeRange : BaseEntity
{
    /// <summary>
    /// Gets or sets the ExceptionDateId
    /// </summary>
    public int ExceptionDateId { get; set; }

    /// <summary>
    /// Gets or sets the AvailableDeliveryTimeRangeId
    /// </summary>
    public int AvailableDeliveryTimeRangeId { get; set; }

    /// <summary>
    /// Gets or sets the Delivery Date On Utc
    /// </summary>
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the Start Date On Utc
    /// </summary>
    public DateTime StartDateOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the End Date On Utc
    /// </summary>
    public DateTime? EndDateOnUtc { get; set; }
}
