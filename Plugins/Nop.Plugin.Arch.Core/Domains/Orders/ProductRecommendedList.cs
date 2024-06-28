using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Represents a product RecommendedList mapping
/// </summary>
public class ProductRecommendedList : BaseEntity, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the RecommendedList identifier
    /// </summary>
    public int RecommendedListId { get; set; }

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
}