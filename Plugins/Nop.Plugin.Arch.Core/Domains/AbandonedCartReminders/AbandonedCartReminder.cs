using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders;
public class AbandonedCartReminder : BaseEntity, IStoreMappingSupported
{
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the delay before sending message
    /// </summary>
    public int? DelayBeforeSend { get; set; }

    /// <summary>
    /// Gets or sets the period of message delay 
    /// </summary>
    public int DelayPeriodId { get; set; }

    /// <summary>
    /// Gets or sets the period of message delay
    /// </summary>
    public MessageDelayPeriod DelayPeriod
    {
        get => (MessageDelayPeriod)DelayPeriodId;
        set => DelayPeriodId = (int)value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    public bool Active { get; set; }
}
