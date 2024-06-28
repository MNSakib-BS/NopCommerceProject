using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class OrderNoteAdditional:BaseEntity
{
    public int OrderNoteId { get; set; }
    /// <summary>
    /// Gets or sets the parent note identifier
    /// </summary>
    public int? ParentId { get; set; }


    /// <summary>
    /// Gets or sets an order note type identifier
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the created by customer id
    /// </summary>
    public int? CreatedByCustomerId { get; set; }

    /// <summary>
    /// Gets or sets the order note type
    /// </summary>
    public OrderNoteType Type
    {
        get => (OrderNoteType)TypeId;
        set => TypeId = (int)value;
    }
}
