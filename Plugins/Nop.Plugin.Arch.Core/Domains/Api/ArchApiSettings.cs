using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Api;
public class ArchApiSettings : ISettings
{
    public bool DisableCertValidationAuth { get; set; }
    public bool AutoSyncEnabled { get; set; }
    public bool AutoPublishProductsNotOnHold { get; set; }
    public string SendTimeout { get; set; }
    public string ReceiveTimeout { get; set; }
    public DateTime Epoch { get; set; }
    public string ApiEndpointAddress { get; set; }
    public string ApiArchMasterDataAddress { get; set; }
    public string SystemAuthenticationCode { get; set; }
    public string DefaultDebtorCode { get; set; }
    public int ExpireCashbackAfterDays { get; set; }
    public int FetchCashbackFromDaysAgo { get; set; }
    public decimal MinimumAmountAllowedForPaymentGatewayRefund { get; set; }
    public bool TrackInventory { get; set; }
    public bool DisplayStockAvailability { get; set; }
    public bool DisplayStockQuantity { get; set; }
    public int OrderMaximumQuantityDefault { get; set; }
    public string ProductImagesFilePath { get; set; }
    public bool DisableCategoryImport { get; set; }
    public bool UseGlobalStoreForCategories { get; set; }

    public ArchApiSettings()
    {
        // Set defaults for Arch
        TrackInventory = true;
        DisplayStockAvailability = true;
    }
}
