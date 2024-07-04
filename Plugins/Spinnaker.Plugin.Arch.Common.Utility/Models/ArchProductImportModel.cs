using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Models
{


    public class ArchProductImportModel
    {
        public List<ArchProductModel> Products { get; set; }
        public ArchProductImportModel()
        {
            Products = new List<ArchProductModel>();
        }
    }

    public record ArchProductModel : BaseNopEntityModel
    {
        public string BaseCode { get; set; }
        public string ProductCode { get; set; }
        public string FullDescription { get; set; }
        public decimal SellingPriceIncl { get; set; }
        public bool Deleted { get; set; }
        public bool OnPromotion { get; set; }
        public string BrandName { get; set; }
        public int DepartmentNumber { get; set; }
        public string DepartmentName { get; set; }
        public bool Taxable { get; set; }
        public decimal TaxRate { get; set; }
        public string PackUnitSize { get; set; }
        public bool POSItem { get; set; }
        public bool Discontinued { get; set; }
        public DateTime PriceActivation { get; set; }
        public DateTime PriceDeactivation { get; set; }
        public decimal NormalPriceIncl { get; set; }
        public int MinStockQty { get; set; }
        public decimal AvailablePackQuantity { get; set; }
        public decimal AvailableUnitQuantity { get; set; }
        public int PackSize { get; set; }
        public string UnitOfMeassure { get; set; }
        public decimal UnitSize { get; set; }
        public bool KeyLine { get; set; }
        public bool KVI { get; set; }
        public int PromotionGroup { get; set; }
        public bool TradeOnline { get; set; }
        public bool HouseBrand { get; set; }
        public decimal SellingPriceInclPrice1 { get; set; }
        public decimal SellingPriceInclPrice2 { get; set; }
        public decimal SellingPriceInclPrice3 { get; set; }
        public decimal SellingPriceInclPrice4 { get; set; }
        public decimal SellingPriceInclPrice5 { get; set; }
        public string BaseDescription { get; set; }
        public string PackDescription { get; set; }
        public bool IsMasterProduct { get; set; }
    }

}
