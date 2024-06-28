using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Represents an order note type enumeration
/// </summary>
public enum OrderNoteType
{
    /// <summary>
    /// General
    /// </summary>
    General = 0,

    /// <summary>
    /// Order Detail Change
    /// </summary>
    Change = 1,

    /// <summary>
    /// Order change response
    ChangeResponse = 2,

    /// <summary>
    /// Payment status changed
    /// </summary>
    Payment = 3,

    /// <summary>
    /// Payment Error
    /// </summary>
    PaymentError = 4
}
