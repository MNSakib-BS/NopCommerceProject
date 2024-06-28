using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class ArchQuotationItem : BaseEntity
{
    public decimal TransactionTrackingNumber { get; set; }
    public int StoreID { get; set; }
    public int CustomerID { get; set; }
}
