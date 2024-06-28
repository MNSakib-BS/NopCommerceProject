using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class OrderAdditional:BaseEntity
{
    public int OrderId { get; set; }
    /// <summary>
    /// Total Fee for variable weight items
    /// </summary>
    public decimal VariableWeightItemReserve { get; set; }

    /// <summary>
    /// Amount deducted from the order from the customer wallet
    /// </summary>
    public decimal? CustomerWalletDeduction { get; set; }

    /// <summary>
    /// Amount paid using customer Debtor Account
    /// </summary>
    public decimal? PaidOnAccount { get; set; }
    public string? SalesOrderBarcode { get; set; }
    public decimal TransactionTrackingNumber { get; set; }
    public int ShoppingBagQuantity { get; set; }
    public decimal ShoppingBagTotalCost { get; set; }
    public bool ArchValidated { get; set; }
    public decimal? OrderTrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the driver (customer) Id for which this order has been allocated to
    /// </summary>
    public int? AllocatedToDriverId { get; set; }

}
