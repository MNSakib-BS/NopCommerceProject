using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class ProductCategoryAdditional : BaseEntity, IStoreMappingSupported
{
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Indicates whether a ProductCategory is scheduled for potential deletion
    /// </summary>
    public bool? IsTombstoned { get; set; }
}
