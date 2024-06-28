using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Arch.Core.Domains.Shipping;

namespace Nop.Plugin.Arch.Core.Services.Shipping;
public interface IShipmentAdditionalService
{
    Task<ShipmentAdditional> GetShipmentAdditionalByShipmentIdAsync(int shipmentId);
    Task<ShipmentAdditional> GetShipmentAdditionalByIdAsync(int id);
    Task DeleteShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional);
    Task InsertShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional);
    Task UpdateShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional);
    /// <summary>
    /// Search shipments
    /// </summary>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <param name="warehouseId">Warehouse identifier, only shipments with products from a specified warehouse will be loaded; 0 to load all orders</param>
    /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
    /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
    /// <param name="shippingCounty">Shipping county; null to load all records</param>
    /// <param name="shippingCity">Shipping city; null to load all records</param>
    /// <param name="trackingNumber">Search by tracking number</param>
    /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
    /// <param name="loadNotDelivered">A value indicating whether we should load only not delivered shipments</param>
    /// <param name="orderId">Order identifier; 0 to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="onlyShowSelectedStore">Select whether to ignore shipments with no store limitations on filter</param>
    /// <returns>Shipments</returns>
    Task<IPagedList<Shipment>> GetAllShipmentsAsync(int vendorId = 0, int warehouseId = 0,
        int shippingCountryId = 0,
        int shippingStateId = 0,
        string shippingCounty = null,
        string shippingCity = null,
        string trackingNumber = null,
        bool loadNotShipped = false,
        bool loadNotDelivered = false,
        int orderId = 0,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue, int storeId = 0, bool onlyShowSelectedStore = false);

}
