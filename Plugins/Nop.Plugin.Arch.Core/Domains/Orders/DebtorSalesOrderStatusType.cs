using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Represents an arch debtors sales order status type
/// </summary>
public enum DebtorSalesOrderStatusType
{
    Pending = 1,
    Picked = 2,
    Dispatched = 3,
    Cancelled = 4,
    Quotation = 5
}