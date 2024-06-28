using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Promotions;
/// <summary>
/// Represents a PromotionGroup template
/// </summary>
public partial class PromotionGroupTemplate : BaseEntity, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the template name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the view path
    /// </summary>
    public string? ViewPath { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }
}