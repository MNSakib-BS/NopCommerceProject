using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Represents an order note change type enumeration
/// </summary>
public enum OrderNoteChangeType
{
    /// <summary>
    /// No Change on order note change identifier
    /// </summary>
    NoChange = 0,

    /// <summary>
    /// Shipping address on order note change identifier
    /// </summary>
    ShippingAddressChange = 1,
}
