using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Models.Catalogs;
public class ArchProductStockOnHandModel
{
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
    public int StockOnHand { get; set; }
}
