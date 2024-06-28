using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class ProductAttributeMappingAdditional:BaseEntity
{
    public int ProductAttributeMappingId { get; set; }
    /// <summary>
    /// Indicates whether a ProductAttributeMapping is scheduled for potential deletion
    /// </summary>
    public bool? IsTombstoned { get; set; }
}
