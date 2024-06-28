using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class ArchProductDetail : BaseEntity
{
    public string? BaseCode { get; set; }
    public string? BaseDescription { get; set; }
    public string? BrandName { get; set; }
    public string? DepartmentName { get; set; }
    public int DepartmentNumber { get; set; }
    public decimal Depth { get; set; }
    public string? FullDescription { get; set; }
    public decimal Height { get; set; }
    public bool HouseBrand { get; set; }
    public bool IsMasterProduct { get; set; }
    public bool KVI { get; set; }
    public bool KeyLine { get; set; }
    public decimal Mass { get; set; }
    public int MinStockQty { get; set; }
    public decimal NormalPriceIncl { get; set; }
    public string? PackDescription { get; set; }
    public decimal PackSize { get; set; }
    public string? PackUnitSize { get; set; }
    public DateTime PriceActivation { get; set; }
    public DateTime PriceDeactivation { get; set; }
    public string? PriceLinkCode { get; set; }
    public string? ProductCode { get; set; }
    public decimal TaxRate { get; set; }
    public bool Taxable { get; set; }
    public string? UnitOfMeassure { get; set; }
    public float UnitSize { get; set; }
    public int WeightType { get; set; }
    public decimal Width { get; set; }
    public DateTime LastUpdated { get; set; }
    public int StoreTypeId { get; set; }
}
