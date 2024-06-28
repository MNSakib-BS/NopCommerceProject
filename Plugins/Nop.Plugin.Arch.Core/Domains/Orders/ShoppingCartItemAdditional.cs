using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class ShoppingCartItemAdditional:BaseEntity
{
    public int ShoppingCartItemId { get; set; }
    /// <summary>
    /// Gets or sets the WishList Group identifier
    /// </summary>
    public int? WishListGroupId { get; set; }
    /// <summary>
    /// Gets or sets the Order Tracking Number from Arch
    /// </summary>
    public decimal? OrderTrackingNumber { get; set; }
}
