using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public enum CustomPaymentStatus
{
    /// <summary>
    /// Pending
    /// </summary>
    Pending = 10,

    /// <summary>
    /// Authorized
    /// </summary>
    Authorized = 20,

    /// <summary>
    /// Paid
    /// </summary>
    Paid = 30,

    /// <summary>
    /// Partially Refunded
    /// </summary>
    PartiallyRefunded = 35,

    /// <summary>
    /// Refunded
    /// </summary>
    Refunded = 40,

    /// <summary>
    /// Voided
    /// </summary>
    Voided = 50,

    /// <summary>
    /// Failed
    /// </summary>
    Failed = 60,
}
