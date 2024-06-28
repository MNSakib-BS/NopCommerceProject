using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public class CustomerWallet : BaseEntity, IStoreMappingSupported
{
    public int CustomerId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedDateUtc { get; set; }

    public DateTime ModifiedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }
}
