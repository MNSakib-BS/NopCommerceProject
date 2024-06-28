using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Models.Catalogs;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IArchStoreProductInfoService
{
    Task<IQueryable<Product>> FilterProductsAsync(IQueryable<Product> query, int storeId = 0, bool loadAll = false);
    Task<ArchStoreProductInfo> GetStoreProductInfoAsync(string productCode, int storeId = 0);
    Task UpdateInHoldingAsync(string productCode, bool isInHolding, int storeId = 0);
    Task UpdateInfoAsync(ArchStoreProductInfo archStoreProductInfo);
    Task<ArchProductPriceModel> GetStorePricesForProductAsync(string productCode, int storeId = 0);
    Task<int> GetStockOnHandAsync(string productCode, int storeId = 0);
    Task<IEnumerable<ArchProductStockOnHandModel>> GetStockOnHandForProductsAsync(IEnumerable<int> productIds, int storeId = 0, int storeTypeId = 0);
    Task UpdateStockOnHandAsync(decimal availableUnitQty, decimal availablePackQty, decimal stockOnHand, string productCode, int storeId);
    Task CalculateInHoldingAsync(string productCodeField, bool productHasPicture, bool productHasCategory, int activeStoreScopeConfiguration);
    Task<IQueryable<Product>> ToArchProductAsync(int storeId);
    Task<IQueryable<Product>> ToArchProductAsync(IQueryable<Product> query, int storeId);
    Task<IEnumerable<ProductPacksModel>> GetProductPacksAsync(IEnumerable<string> baseCodes, int storeId = 0, int storeTypeId = 0);
    Task<ProductPacksModel> GetProductBasePackPriceDetailAsync(string baseCode, int storeId = 0, int storeTypeId = 0);
    Task<IEnumerable<ProductPacksModel>> GetProductBasePackPriceDetailsAsync(IEnumerable<string> baseCodes, int storeId = 0, int storeTypeId = 0);
    Task<IQueryable<Category>> FilterCategoriesAsync(IQueryable<Category> query, int storeId = 0, int storeTypeId = 0);
}

