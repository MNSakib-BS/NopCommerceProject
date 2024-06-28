using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Models.Catalogs;
public class ArchProductPriceModel
{
    public decimal SellingPriceIncl { get; set; }

    public decimal SellingPriceInclPrice1 { get; set; }

    public decimal SellingPriceInclPrice2 { get; set; }

    public decimal SellingPriceInclPrice3 { get; set; }

    public decimal SellingPriceInclPrice4 { get; set; }

    public decimal SellingPriceInclPrice5 { get; set; }

    public bool IsInHolding { get; set; }
}
