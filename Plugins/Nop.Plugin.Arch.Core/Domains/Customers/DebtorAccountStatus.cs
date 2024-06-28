using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public enum DebtorAccountStatus
{
    Good = 0,
    Fair = 1,
    Bad = 2,
    Closed = 3,
    HoldOnOverdue = 4,
    OnHold = 5,
}
