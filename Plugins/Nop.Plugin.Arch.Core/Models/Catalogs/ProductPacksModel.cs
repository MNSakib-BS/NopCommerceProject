using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Models.Catalogs;
public class ProductPacksModel
{
    public string? ProductCode { get; internal set; }
    public string? BaseCode { get; internal set; }
    public decimal SellingPrice { get; internal set; }
    public int? PackSize { get; internal set; }
    public int StoreTypeId { get; internal set; }
    public int StoreId { get; internal set; }
}
