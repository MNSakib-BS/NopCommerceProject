using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Arch.Core.Domains.Common;

namespace Nop.Plugin.Arch.Core.Domains.Stores;
public class StoreAdditional : BaseEntity
{
    public int StoreId { get; set; }
    /// <summary>
    /// True if the store is the global store. Otherwise; False.
    /// </summary>
    public bool IsGlobalStore { get; set; }

    /// <summary>
    /// True if the store supports delivery. Otherwise; False.
    /// </summary>
    public bool IsDeliveryAvailable { get; set; }

    /// <summary>
    /// True if the store supports collection. Otherwise; False.
    /// </summary>
    public bool IsCollectAvailable { get; set; }

    /// <summary>
    /// True if the store's phone number should show on the contact us page.
    /// </summary>
    public bool ShowNumberOnContactUs { get; set; }
    public string? StoreLocatorName { get; set; }

    /// <summary>
    /// Gets or sets the company address latitude
    /// </summary>
    public decimal Latitude { get; set; }

    /// <summary>
    /// Gets or sets the company address longitude
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the default country for this store; 0 is set when we use the default country display order
    /// </summary>
    public int DefaultCountryId { get; set; }
    /// <summary>
    /// Gets or sets the store mobile number used to send SMS from / to the store
    /// </summary>
    public string? StoreMobileNumber { get; set; }
    public string? StoreCode { get; set; }
    #region Arch        

    /// Gets or sets the variable weight item markup percentage
    /// </summary>
    public decimal VariableWeightItemMarkupPercent { get; set; }

    /// Gets or sets the shopping bag product code
    /// </summary>
    public string? ShoppingBagProductCode { get; set; }

    /// Gets or sets the delivery fee product code
    /// </summary>
    public string? DeliveryFeeProductCode { get; set; }

    /// <summary>
    /// Gets or sets the default price tier
    /// </summary>
    public int DefaultPriceTierId { get; set; }

    public double? DistanceInKilometers { get; set; }

    public double? MaxRadius { get; set; }

    /// <summary>
    /// Gets or sets the default price tier type
    /// </summary>
    public ArchPriceTierType DefaultPriceTier
    {
        get => (ArchPriceTierType)DefaultPriceTierId;
        set => DefaultPriceTierId = (int)value;
    }

    #endregion
}
