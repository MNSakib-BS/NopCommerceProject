using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class PredefinedProductAttributeValueAdditional:BaseEntity
{
    public int PredefinedProductAttributeValueId { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether "price adjustment" is displayed
    /// </summary>
    public bool DisplayPriceAdjustment { get; set; }
}
