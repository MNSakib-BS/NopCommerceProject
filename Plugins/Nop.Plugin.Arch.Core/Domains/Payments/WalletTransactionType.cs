using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public enum WalletTransactionType
{
    Info = 0,
    Credit = 1,
    Debit = 2,
}
