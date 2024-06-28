using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Office;
using LinqToDB;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Models.Catalogs;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class ArchStoreProductInfoService : IArchStoreProductInfoService
{
    #region Fields

    private readonly IRepository<ArchStoreProductInfo> _storeInfoRepository;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<StoreType> _storeTypeRepository;
    private readonly IRepository<StoreTypeMapping> _storeTypeMappingRepository;
    private readonly IRepository<ProductAdditional> _productAdditionalRepository;
    private readonly IRepository<CategoryAdditional> _categoryAdditionalRepository;

    #endregion

    #region Ctor

    public ArchStoreProductInfoService(IRepository<ArchStoreProductInfo> storeInfoRepository,
       ICacheKeyService cacheKeyService,
       IEventPublisher eventPublisher,
       IRepository<Product> productRepository,
       IRepository<StoreType> storeTypeRepository,
       IRepository<StoreTypeMapping> storeTypeMappingRepository,
       IRepository<ProductAdditional> productAdditionalRepository,
       IRepository<CategoryAdditional> categoryAdditionalRepository)
    {
        _storeInfoRepository = storeInfoRepository;
        _cacheKeyService = cacheKeyService;
        _eventPublisher = eventPublisher;
        _productRepository = productRepository;
        _storeTypeRepository = storeTypeRepository;
        _storeTypeMappingRepository = storeTypeMappingRepository;
        _productAdditionalRepository = productAdditionalRepository;
        _categoryAdditionalRepository = categoryAdditionalRepository;
    }

    #endregion

    #region Utitlies

    private IQueryable<ProductPacksModel> GetProductPackQueryable(int storeId, int storeTypeId)
    {
        var query = from product in _productRepository.Table
                    join pa in _productAdditionalRepository.Table.Where(p => p.StoreTypeId == storeTypeId) on product.Id equals pa.ProductId 
                    join archProduct in _storeInfoRepository.Table on
                    new { ProductCode = pa.ProductCodeField, StoreTypeId = pa.StoreTypeId }
                    equals
                    new { ProductCode = archProduct.ProductCode, StoreTypeId = archProduct.StoreTypeId }
                    select new
                    {
                        ProductCode = pa.ProductCodeField,
                        BaseCode = pa.BaseCodeField,
                        PackSize = pa.PackSizeField,
                        StoreId = archProduct.StoreId,
                        StoreTypeId = archProduct.StoreTypeId,
                        SellingPrice = archProduct.SellingPriceIncl,
                    };

        if (storeId > 0)
        {
            query = query.Where(p => p.StoreId == storeId);
        }

        var result = from item in query
                     select new ProductPacksModel
                     {
                         ProductCode = item.ProductCode,
                         BaseCode = item.BaseCode,
                         SellingPrice = item.SellingPrice,
                         PackSize = (int)item.PackSize,
                         StoreTypeId = item.StoreTypeId,
                         StoreId = item.StoreId
                     };

        return result;
    }

    #endregion

    #region Methods

    public virtual async Task<IQueryable<Product>> FilterProductsAsync(IQueryable<Product> query, int storeId = 0, bool loadAll = false)
    {
        var storeTypeId =  (from sm in _storeTypeMappingRepository.Table.Where(p => p.StoreId == storeId)
                                 join st in _storeTypeRepository.Table on sm.StoreTypeId equals st.Id
                                 select st.Id).FirstOrDefault();

       

        var productAdditionalQuery = _productAdditionalRepository.Table;

        var holdQuery = from item in _storeInfoRepository.Table.Where(p => p.StoreId == storeId)
                        select new
                        {
                            item.ProductCode,
                            item.IsInHolding
                        };

        if (!loadAll)
            query = from entity in query
                    join pa in productAdditionalQuery.Where(p => holdQuery.Any(a => a.ProductCode == p.ProductCodeField && !a.IsInHolding)) on entity.Id equals pa.ProductId
                    select entity;
        else
            query = from entity in query
                    join pa in productAdditionalQuery.Where(p => holdQuery.Any(a => a.ProductCode == p.ProductCodeField))
                    on entity.Id equals pa.ProductId
                    select entity;

        if (storeTypeId > 0)
            query = from p in query
                    join pa in productAdditionalQuery.Where(p => p.StoreTypeId == storeTypeId)
                    on p.Id equals pa.ProductId
                    select p;

        return query;
    }

    public virtual async Task<IQueryable<Category>> FilterCategoriesAsync(IQueryable<Category> query, int storeId = 0, int storeTypeId = 0)
    {
        if (storeTypeId == 0 && storeId > 0)
        {
            storeTypeId =  (from sm in _storeTypeMappingRepository.Table.Where(p => p.StoreId == storeId)
                                 join st in _storeTypeRepository.Table on sm.StoreTypeId equals st.Id
                                 select st.Id).FirstOrDefault();
        }

        if (storeTypeId == 0)
            throw new ArgumentException("Store Type ID cannot be 0");

        if (storeTypeId > 0)
            query = from c in query
                    join ca in _categoryAdditionalRepository.Table on c.Id equals ca.CategoryId
                    where ca.StoreTypeId == storeTypeId
                    select c;

        return query;
    }

    public virtual async Task<IQueryable<Product>> ToArchProductAsync(int storeId)
    {
        return await ToArchProductAsync(_productRepository.Table, storeId);
    }

    public virtual async Task<IQueryable<Product>> ToArchProductAsync(IQueryable<Product> query, int storeId)
    {
        var productsQuery = from product in query
                            join pa in _productAdditionalRepository.Table on product.Id equals pa.ProductId
                            join aspi in _storeInfoRepository.Table.Where(p => p.StoreId == storeId) on
                                 new { ProductCode = pa.ProductCodeField, StoreTypeId = pa.StoreTypeId }
                            equals
                                 new { ProductCode = aspi.ProductCode, StoreTypeId = aspi.StoreTypeId }
                            select new Product
                            {
                                Id = product.Id,
                                Name = product.Name,
                                ShortDescription = product.ShortDescription,
                                FullDescription = product.FullDescription,
                                MetaTitle = product.MetaTitle,
                                MetaDescription = product.MetaDescription,
                                MetaKeywords = product.MetaKeywords,
                                Gtin = product.Gtin,
                                Sku = product.Sku,
                                OldPrice = product.OldPrice,
                                Price = aspi.SellingPriceIncl,
                                DisableBuyButton = product.DisableBuyButton,
                                DisableWishlistButton = product.DisableWishlistButton,
                                AvailableForPreOrder = product.AvailableForPreOrder,
                                PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
                                ManufacturerPartNumber = product.ManufacturerPartNumber,
                                //StoreTypeId = pa.StoreTypeId,
                                Published = !(aspi.SellingPriceIncl > 0 && aspi.POSItem && aspi.TradeOnline) ? false : !aspi.IsInHolding,
                                Deleted = product.Deleted,
                                //ProductCodeField = pa.ProductCodeField,
                                VendorId = product.VendorId,
                                VisibleIndividually = product.VisibleIndividually,
                                WarehouseId = product.WarehouseId,
                                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
                                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                                DisplayOrder = product.DisplayOrder,
                                //PromotionGroupField = pa.PromotionGroupField,
                                //OnPromotionField = aspi.OnPromotion,
                                StockQuantity = (int)aspi.AvailablePackQuantity,
                                ProductTypeId = product.ProductTypeId,
                                ManageInventoryMethodId = product.ManageInventoryMethodId,
                                //AvailablePackQuantityField = aspi.AvailablePackQuantity,
                                //AvailableUnitQuantityField = aspi.AvailableUnitQuantity,
                                //PackSizeField = pa.PackSizeField,
                                //BaseCodeField = pa.BaseCodeField,
                            };

        return await Task.FromResult(productsQuery);
    }

    public virtual async Task UpdateInHoldingAsync(string productCode, bool isInHolding, int storeId = 0)
    {
        await _storeInfoRepository.Table.Where(p => p.ProductCode == productCode && p.StoreId == storeId)
            .Set(p => p.IsInHolding, isInHolding)
            .UpdateAsync();
    }

    public virtual async Task<ArchStoreProductInfo> GetStoreProductInfoAsync(string productCode, int storeId = 0)
    {
        var key = $"ArchStoreProduct-{productCode}-{storeId}";
        return await _storeInfoRepository.Table.FirstOrDefaultAsync(p => p.ProductCode == productCode && p.StoreId == storeId);
    }

    public virtual async Task UpdateInfoAsync(ArchStoreProductInfo archStoreProductInfo)
    {
        await _storeInfoRepository.UpdateAsync(archStoreProductInfo);
        await _eventPublisher.EntityUpdatedAsync(archStoreProductInfo);
    }

    public virtual async Task<ArchProductPriceModel> GetStorePricesForProductAsync(string productCode, int storeId = 0)
    {
        var key = $"ArchStorePrice-{productCode}-{storeId}";

        var storePrice =  (from price in _storeInfoRepository.Table.Where(p => p.StoreId == storeId && p.ProductCode == productCode)
                                select new ArchProductPriceModel
                                {
                                    SellingPriceIncl = price.SellingPriceIncl,
                                    SellingPriceInclPrice1 = price.SellingPriceInclPrice1,
                                    SellingPriceInclPrice2 = price.SellingPriceInclPrice2,
                                    SellingPriceInclPrice3 = price.SellingPriceInclPrice3,
                                    SellingPriceInclPrice4 = price.SellingPriceInclPrice4,
                                    SellingPriceInclPrice5 = price.SellingPriceInclPrice5,
                                    IsInHolding = price.IsInHolding

                                }).FirstOrDefault();

        return storePrice;
    }

    public virtual async Task<int> GetStockOnHandAsync(string productCode, int storeId = 0)
    {
        var onHandQuery = from product in _storeInfoRepository.Table.Where(p => p.ProductCode == productCode)
                          select new
                          {
                              product.ProductCode,
                              product.AvailableUnitQuantity,
                              product.AvailablePackQuantity,
                              product.StoreId
                          };

        if (storeId > 0)
            onHandQuery = onHandQuery.Where(p => p.StoreId == storeId);

        var results = await onHandQuery.ToListAsync();
        var onHand = (int)results.Sum(p => p.AvailablePackQuantity);
        return onHand;
    }

    public virtual async Task<IEnumerable<ArchProductStockOnHandModel>> GetStockOnHandForProductsAsync(IEnumerable<int> productIds, int storeId = 0, int storeType = 0)
    {
        var productQuery = from product in _productRepository.Table
                           join pa in _productAdditionalRepository.Table on product.Id equals pa.ProductId
                           where pa.StoreTypeId == storeType && productIds.Contains(product.Id)
                           select product;

        var onHandQuery = from product in _storeInfoRepository.Table
                          select new
                          {
                              product.ProductCode,
                              product.AvailableUnitQuantity,
                              product.AvailablePackQuantity,
                              product.StoreId
                          };

        if (storeId > 0)
        {
            onHandQuery = onHandQuery.Where(p => p.StoreId == storeId);
        }

        var results = from product in productQuery
                      join pa in _productAdditionalRepository.Table on product.Id equals pa.ProductId
                      join item in onHandQuery on pa.ProductCodeField equals item.ProductCode
                      select new ArchProductStockOnHandModel
                      {
                          ProductCode = item.ProductCode,
                          ProductId = product.Id,
                          StockOnHand = (int)item.AvailablePackQuantity
                      };
        return await results.ToListAsync();
    }

    public virtual async Task UpdateStockOnHandAsync(decimal availableUnitQty, decimal availablePackQty, decimal stockOnHand, string productCode, int storeId)
    {
        var storeInfo = await _storeInfoRepository.Table.FirstOrDefaultAsync(p => p.ProductCode == productCode && p.StoreId == storeId);
        if (storeInfo != null)
        {
            storeInfo.AvailablePackQuantity = availablePackQty;
            storeInfo.AvailableUnitQuantity = availableUnitQty;
            storeInfo.StockOnHandField = stockOnHand;
            await _storeInfoRepository.UpdateAsync(storeInfo);
        }
    }

    public virtual async Task CalculateInHoldingAsync(string productCodeField, bool productHasPicture, bool productHasCategory, int activeStoreScopeConfiguration)
    {
        var productInfo = (from archProductInfo in _storeInfoRepository.Table.Where(p => p.ProductCode == productCodeField && p.StoreTypeId == activeStoreScopeConfiguration)
                                 select new
                                 {
                                     archProductInfo.ProductCode,
                                     archProductInfo.POSItem,
                                     archProductInfo.TradeOnline,
                                     archProductInfo.SellingPriceIncl,
                                     archProductInfo.Deleted,
                                     archProductInfo.StoreId
                                 }).FirstOrDefault();
       

        if (productInfo == null)
            return;

        var allowedToSellOnline = productInfo.POSItem && productInfo.TradeOnline && productInfo.SellingPriceIncl > 0 && !productInfo.Deleted;
        var isInHolding = !allowedToSellOnline || !productHasCategory || !productHasPicture;

        await _storeInfoRepository.Table.Where(p => p.ProductCode == productInfo.ProductCode && p.StoreId == productInfo.StoreId)
            .Set(p => p.IsInHolding, isInHolding)
            .UpdateAsync();
    }

    public virtual async Task<IEnumerable<ProductPacksModel>> GetProductPacksAsync(IEnumerable<string> baseCodes, int storeId = 0, int storeTypeId = 0)
    {
        var result = await GetProductPackQueryable(storeId, storeTypeId)
            .Where(p => baseCodes.Any(a => a == p.BaseCode))
            .ToListAsync();

        return result;
    }

    public async Task<ProductPacksModel> GetProductBasePackPriceDetailAsync(string baseCode, int storeId = 0, int storeTypeId = 0)
    {
        var query = GetProductPackQueryable(storeId, storeTypeId);

        var result =  query.Where(p => p.BaseCode == baseCode && p.SellingPrice > 0 && p.PackSize == 1)
            .OrderBy(p => p.SellingPrice)
            .FirstOrDefault();

        return result;
    }

    public async Task<IEnumerable<ProductPacksModel>> GetProductBasePackPriceDetailsAsync(IEnumerable<string> baseCodes, int storeId = 0, int storeTypeId = 0)
    {
        if (!baseCodes.Any())
            return new List<ProductPacksModel>();

        var query = GetProductPackQueryable(storeId, storeTypeId);

        var result = await query.Where(p => baseCodes.Any(a => a == p.BaseCode) && p.SellingPrice > 0 && p.PackSize == 1)
            .OrderBy(p => p.SellingPrice)
            .ToListAsync();

        return result;
    }

    #endregion

}



