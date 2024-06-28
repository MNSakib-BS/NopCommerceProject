using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Promotions;
public class PromotionGroupWidgetZones : BaseEntity, IStoreMappingSupported
{
    public int PromotionGroupId { get; set; }

    public string? WidgetZones { get; set; }

    public bool LimitedToStores { get; set; }
}
