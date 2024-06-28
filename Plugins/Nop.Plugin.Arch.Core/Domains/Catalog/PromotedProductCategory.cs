using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
/// <summary>
/// Represents a product category mapping
/// </summary>
public  class PromotedProductCategory : BaseEntity, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the category identifier
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is featured
    /// </summary>
    public bool IsFeaturedProduct { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Indicates whether a ProductCategory is scheduled for potential deletion
    /// </summary>
    public bool? IsTombstoned { get; set; }
}
