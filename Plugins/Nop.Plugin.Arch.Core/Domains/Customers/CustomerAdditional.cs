using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public class CustomerAdditional:BaseEntity,IStoreMappingSupported
{
    public int CustomerId { get; set; }
    /// <summary>
    /// Gets or sets the Loyalty Card Number
    /// </summary>
    public string LoyaltyCardNumber { get; set; }
    /// <summary>
    ///  Gets or sets the default store identifier associated with the customer
    /// </summary>
    public int DefaultStoreId { get; set; }

    /// <summary>
    ///  Hide message to customer to set default store if True; Otherwise, False.
    /// </summary>
    public bool HideSetDefaultStoreMessage { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public virtual bool LimitedToStores { get; set; }
}
