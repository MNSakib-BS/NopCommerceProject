using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class ArchStoreProductInfo : BaseEntity, IStoreMappingSupported
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; }
    public int StoreId { get; set; }
    public bool Discontinued { get; set; }
    public bool Deleted { get; set; }

    public bool TradeOnline { get; set; }

    public decimal SellingPriceIncl { get; set; }

    public decimal SellingPriceInclPrice1 { get; set; }

    public decimal SellingPriceInclPrice2 { get; set; }

    public decimal SellingPriceInclPrice3 { get; set; }

    public decimal SellingPriceInclPrice4 { get; set; }

    public decimal SellingPriceInclPrice5 { get; set; }

    public DateTime LastUpdated { get; set; }

    public bool OnPromotion { get; set; }

    public bool POSItem { get; set; }

    public int PromotionGroup { get; set; }

    public decimal AvailablePackQuantity { get; set; }

    public decimal AvailableUnitQuantity { get; set; }

    public bool IsInHolding { get; set; }

    public decimal StockOnHandField { get; set; }

    public bool LimitedToStores { get; set; }

    public int StoreTypeId { get; set; }

    public Guid? UpdateSessionID { get; set; }
}
