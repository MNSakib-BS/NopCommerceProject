using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Shipping;
using Nop.Plugin.Arch.Core.Services.Stores;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Shipping;
public class ShipmentAdditionalService : IShipmentAdditionalService
{
    #region Fields

    private readonly IRepository<ShipmentAdditional> _shipmentAdditionalRepository;
    private readonly IRepository<Shipment> _shipmentRepository;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreMappingAdditionalService _storeMappingAdditionalService;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ShipmentItem> _siRepository;

    #endregion

    #region Ctor

    public ShipmentAdditionalService(IRepository<ShipmentAdditional> shipmentAdditionalRepository,
        IRepository<Shipment> shipmentRepository,
        IStoreMappingService storeMappingService,
        IStoreMappingAdditionalService storeMappingAdditionalService,
        IRepository<Order> orderRepository,
        IRepository<Address> addressRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Product> productRepository,
        IRepository<ShipmentItem> siRepository)
    {
        _shipmentAdditionalRepository = shipmentAdditionalRepository;
        _shipmentRepository = shipmentRepository;
        _storeMappingService = storeMappingService;
        _storeMappingAdditionalService = storeMappingAdditionalService;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _siRepository = siRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods
    public virtual async Task DeleteShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional)
    {
        await _shipmentAdditionalRepository.DeleteAsync(shipmentAdditional);
    }

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
    public virtual async Task<IPagedList<Shipment>> GetAllShipmentsAsync(int vendorId = 0, int warehouseId = 0,
        int shippingCountryId = 0,
        int shippingStateId = 0,
        string shippingCounty = null,
        string shippingCity = null,
        string trackingNumber = null,
        bool loadNotShipped = false,
        bool loadNotDelivered = false,
        int orderId = 0,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue, int storeId = 0, bool onlyShowSelectedStore = false)
    {
        var query = _shipmentRepository.Table;

        var shipmentAdditionalQuery = from shipment in query
                                      join sa in _shipmentAdditionalRepository.Table on shipment.Id equals sa.ShipmentId
                                      select sa;

        shipmentAdditionalQuery = await _storeMappingAdditionalService.FilterStoresAsync(shipmentAdditionalQuery, storeId, onlyShowSelectedStore);

        query = from shipment in query
                join sa in shipmentAdditionalQuery on shipment.Id equals sa.ShipmentId
                select shipment;

        if (orderId > 0)
            query = query.Where(o => o.OrderId == orderId);

        if (!string.IsNullOrEmpty(trackingNumber))
            query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));

        if (shippingCountryId > 0)
            query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.CountryId == shippingCountryId)
                    select s;

        if (shippingStateId > 0)
            query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.StateProvinceId == shippingStateId)
                    select s;

        if (!string.IsNullOrWhiteSpace(shippingCounty))
            query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.County.Contains(shippingCounty))
                    select s;

        if (!string.IsNullOrWhiteSpace(shippingCity))
            query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.City.Contains(shippingCity))
                    select s;

        if (loadNotShipped)
            query = query.Where(s => !s.ShippedDateUtc.HasValue);

        if (loadNotDelivered)
            query = query.Where(s => !s.DeliveryDateUtc.HasValue);

        if (createdFromUtc.HasValue)
            query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);

        if (createdToUtc.HasValue)
            query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);

        query = from s in query
                join o in _orderRepository.Table on s.OrderId equals o.Id
                where !o.Deleted
                select s;

        query = query.Distinct();

        if (vendorId > 0)
        {
            var queryVendorOrderItems = from orderItem in _orderItemRepository.Table
                                        join p in _productRepository.Table on orderItem.ProductId equals p.Id
                                        where p.VendorId == vendorId
                                        select orderItem.Id;

            query = from s in query
                    join si in _siRepository.Table on s.Id equals si.ShipmentId
                    where queryVendorOrderItems.Contains(si.OrderItemId)
                    select s;

            query = query.Distinct();
        }

        if (warehouseId > 0)
        {
            query = from s in query
                    join si in _siRepository.Table on s.Id equals si.ShipmentId
                    where si.WarehouseId == warehouseId
                    select s;

            query = query.Distinct();
        }

        query = query.OrderByDescending(s => s.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);

    }



    public virtual async Task<ShipmentAdditional> GetShipmentAdditionalByIdAsync(int id)
    {
        return await _shipmentAdditionalRepository.GetByIdAsync(id, cache => default, false);
    }

    public virtual async Task<ShipmentAdditional> GetShipmentAdditionalByShipmentIdAsync(int shipmentId)
    {
        return await _shipmentAdditionalRepository.Table.Where(e => e.ShipmentId == shipmentId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional)
    {
        await _shipmentAdditionalRepository.InsertAsync(shipmentAdditional);
    }

    public virtual async Task UpdateShipmentAdditionalAsync(ShipmentAdditional shipmentAdditional)
    {
        await _shipmentAdditionalRepository.UpdateAsync(shipmentAdditional);
    }

    #endregion
}
