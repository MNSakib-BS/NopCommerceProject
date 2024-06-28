using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class ArchInvoiceItem : BaseEntity
{
    public decimal TransactionTrackingNumber { get; set; }
    public string? DebtorNumber { get; set; }
    public string? LoyaltyCardNumber { get; set; }
    public string? OrderNumber { get; set; }
    public string? ProductCode { get; set; }
    public decimal PackQuantity { get; set; }
    public decimal PackSellingPriceIncl { get; set; }
    public decimal PackSize { get; set; }
    public decimal Quantity { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TotalLineDiscount { get; set; }
    public decimal TotalLineIncl { get; set; }
    public int StoreId { get; set; }
}
