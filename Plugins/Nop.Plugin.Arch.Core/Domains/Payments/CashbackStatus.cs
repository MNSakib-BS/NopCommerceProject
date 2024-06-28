using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
/// <summary>
/// Represents a cashback status enumeration
/// </summary>
public enum CashbackStatus
{
    /// <summary>
    /// NotApplicable
    /// </summary>
    NotApplicable = 0,

    /// <summary>
    /// Pending
    /// </summary>
    Pending = 10,

    /// <summary>
    /// Authorized
    /// </summary>
    Authorized = 20,

    /// <summary>
    /// Partially claimed
    /// </summary>
    PartiallyClaimed = 35,

    /// <summary>
    /// Claimed
    /// </summary>
    Claimed = 40,

    /// <summary>
    /// Voided
    /// </summary>
    Voided = 50,

    /// <summary>
    /// Expired
    /// </summary>
    Expired = 51
}
