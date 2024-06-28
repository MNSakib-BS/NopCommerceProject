using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Stores;
/// <summary>
/// Represents a store mapping record
/// </summary>
public class CustomerWalletStoreMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public int CustomerWalletId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }
}
