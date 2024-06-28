using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class ProductAdditional:BaseEntity
{
    public int ProductId { get; set; }
    /// <summary>
    /// Indicates whether a Product is scheduled for potential deletion
    /// </summary>
    public bool? IsTombstoned { get; set; }
    public string? Awards { get; set; }
    public string? Origin { get; set; }
    public decimal AlcoholByVolume { get; set; }
    public string? BaseDescriptionField { get; set; }
    public decimal DepthField { get; set; }
    public decimal HeightField { get; set; }
    public decimal MassField { get; set; }
    public int WeightTypeField { get; set; }
    public int DepartmentNumberField { get; set; }
    public decimal WidthField { get; set; }
    public string? PackDescriptionField { get; set; }
    public string? PriceLinkCodeField { get; set; }
    public decimal AvailablePackQuantityField { get; set; }
    public decimal AvailableUnitQuantityField { get; set; }
    public decimal? PackSizeField { get; set; }
    public string? UnitOfMeassureField { get; set; }
    public decimal? UnitSizeField { get; set; }
    public string? BaseCodeField { get; set; }
    public string? BrandNameField { get; set; }
    public bool DiscontinuedField { get; set; }
    public DateTime LastUpdatedField { get; set; }
    public bool HouseBrandField { get; set; }
    public bool KVIField { get; set; }
    public bool KeyLineField { get; set; }
    public decimal NormalPriceInclField { get; set; }
    public bool OnPromotionField { get; set; }
    public bool POSItemField { get; set; }
    public string? PackUnitSizeField { get; set; }
    public DateTime PriceActivationField { get; set; }
    public DateTime PriceDeactivationField { get; set; }
    public string? ProductCodeField { get; set; }
    public int PromotionGroupField { get; set; }
    public int ReturnableContainerField { get; set; }
    public decimal SellingPriceInclField { get; set; }
    public decimal SellingPriceInclPrice1Field { get; set; }
    public decimal SellingPriceInclPrice2Field { get; set; }
    public decimal SellingPriceInclPrice3Field { get; set; }
    public decimal SellingPriceInclPrice4Field { get; set; }
    public decimal SellingPriceInclPrice5Field { get; set; }
    public decimal StockOnHandField { get; set; }
    public decimal TaxRateField { get; set; }
    public bool TradeOnlineField { get; set; }
    public int StoreTypeId { get; set; }
    public bool IsInHolding { get; set; }
    public bool DisableZoom { get; set; }
}
