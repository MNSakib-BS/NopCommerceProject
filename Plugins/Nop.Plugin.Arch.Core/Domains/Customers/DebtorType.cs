using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public enum DebtorType
{
    Credit = 0,

    /// <summary>
    /// Debtor can only do cash orders
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Debtor  can  charge to an  account
    /// </summary>
    Exempt = 2,
}
