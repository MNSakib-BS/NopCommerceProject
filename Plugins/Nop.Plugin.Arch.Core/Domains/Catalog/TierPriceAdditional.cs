using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class TierPriceAdditional:BaseEntity
{
    public int TierPriceId { get; set; }

    public decimal SellingPrice2 { get; set; }

    public decimal SellingPrice3 { get; set; }

    public decimal SellingPrice4 { get; set; }

    public decimal SellingPrice5 { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal AvailablePackQuantityField { get; set; }

    public decimal AvailableUnitQuantityField { get; set; }

    public int MinStockQuantity { get; set; }
}
