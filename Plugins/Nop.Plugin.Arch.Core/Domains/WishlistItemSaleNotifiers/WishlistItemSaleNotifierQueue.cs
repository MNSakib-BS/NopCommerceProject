using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.WishlistItemSaleNotifiers;
public class WishlistItemSaleNotifierQueue : BaseEntity
{
    public int CustomerId { get; set; }
    public int StoreId { get; set; }
    public int ReminderId { get; set; }
    public DateTime CartLastUpdated { get; set; }
    public DateTime MessageLastSent { get; set; }
    public bool IsActive { get; set; }
}
