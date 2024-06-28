using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Shipping;
public class ShipmentAdditional: BaseEntity, IStoreMappingSupported
{
    public int ShipmentId { get; set; }
    /// <summary>
    /// Gets or sets the returned date and time
    /// </summary>
    public DateTime? ReturnedDateUtc { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public virtual bool LimitedToStores { get; set; }
}
