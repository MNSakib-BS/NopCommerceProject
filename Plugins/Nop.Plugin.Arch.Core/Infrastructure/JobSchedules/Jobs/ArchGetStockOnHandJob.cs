using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArchServiceReference;
using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Extensions.Logging;
using Arch.Core.Services.Catalog;
using Arch.Core.Services.Helpers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using static iTextSharp.text.pdf.AcroFields;
using ILogger = Nop.Services.Logging.ILogger;

namespace Arch.Core.Infrastructure.JobSchedules.Jobs
{
    public class ArchGetStockOnHandJob : ArchScheduledJobBase<ProductAvailabilityResponse.ProductAvailabilityElement>
    {
        protected override Type TaskType => typeof(ArchGetStockOnHandJob);

        private const string LastUpdateSettingParam = "ArchGetStockOnHandTask_LastUpdate";
        private const string BatchSizeSettingParam = "ArchProductFullList_BatchSize";

        private readonly IProductService _productService;
        protected readonly IArchStoreProductInfoService _archStoreProductInfoService;
        protected readonly IRepository<ArchStoreProductInfo> _productStoreProductInfoRepository;
        protected readonly List<ProductAvailabilityResponse.ProductAvailabilityElement> _productElementList;
        private readonly IRepository<ArchProductDetail> _productDetailRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        private readonly INopDataProvider _dataProvider;
        public ArchGetStockOnHandJob(IProductService productService,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
            ILogger<ScheduledJobBase<object>> jobLogger,
            IArchStoreProductInfoService archStoreProductInfoService,
            INopDataProvider dataProvider)
            : base(settingService,
                storeService,
                storeContext,
                storeMappingService,
                urlRecordService,
                logger,
                objectConverter,
                jobLogger)
        {
            _productService = productService;
            _archStoreProductInfoService = archStoreProductInfoService;
            _productStoreProductInfoRepository = EngineContext.Current.Resolve<IRepository<ArchStoreProductInfo>>();
            _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            _productElementList = new List<ProductAvailabilityResponse.ProductAvailabilityElement>();
            _productDetailRepository = EngineContext.Current.Resolve<IRepository<ArchProductDetail>>();
            _dataProvider = dataProvider;
        }

        protected override void Produce()
        {
            var lastUpdate = GetLastUpdate(LastUpdateSettingParam);
            var take = GetSetting(BatchSizeSettingParam, 1000);
            var skip = 0;
            bool hasRecords = true;
            var isGlobalStore = _storeService.GetStoreById(RunningOnStoreId).IsGlobalStore;
            if (Debugger.IsAttached)
            {
                if (RunningOnStoreId != 1)
                    return;
            }

            List<ArchStoreProductInfo> productStoreInfoList = new List<ArchStoreProductInfo>();
            do
            {

                Debug($"Calling ArchAPI");
                var response = ArchClient.GetProductAvailabilityAsync(new ProductAvailabilityRequest
                {
                    LastUpdate = lastUpdate,
                    SystemAuthenticationCode = _archSettings.SystemAuthenticationCode,
                    ProductCode = "",
                    TakeRecords = take,
                    SkipRecords = skip
                }).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!response.Success)
                {
                    Debug(response.ResponseMessage);
                    return;
                }

                var count = 0;
                if (response.Items != null && response.Items.Any())
                {
                    count = response.Items.Length;
                    Debug($"Producing {count} items");

                    foreach (var item in response.Items)
                    {
                        var storeItem = new ArchStoreProductInfo
                        {
                            Deleted = item.Deleted,
                            Discontinued = item.Discontinued,
                            LastUpdated = lastUpdate,
                            OnPromotion = item.OnPromotion,
                            ProductCode = item.ProductCode,
                            SellingPriceIncl = item.SellingPriceIncl,
                            SellingPriceInclPrice1 = item.SellingPriceInclPrice1,
                            SellingPriceInclPrice2 = item.SellingPriceInclPrice2,
                            SellingPriceInclPrice3 = item.SellingPriceInclPrice3,
                            SellingPriceInclPrice4 = item.SellingPriceInclPrice4,
                            SellingPriceInclPrice5 = item.SellingPriceInclPrice5,
                            POSItem = item.POSItem,
                            StoreId = RunningOnStoreId,
                            TradeOnline = item.TradeOnline,
                            PromotionGroup = item.PromotionGroup,
                            AvailablePackQuantity = item.AvailablePackQuantity,
                            AvailableUnitQuantity = item.AvailableUnitQuantity,
                            StockOnHandField = item.AvailablePackQuantity,
                            StoreTypeId = StoreTypeId,
                            IsInHolding = !(item.TradeOnline && item.POSItem && item.SellingPriceIncl > 0 && !item.Deleted),
                            LimitedToStores = false
                        };

                        productStoreInfoList.Add(storeItem);
                        _productElementList.Add(item);
                        EnqueueItem(item);
                    }
                }

                if (productStoreInfoList.Any())
                {
                    Debug($"Merge Store Product Availability");

                    var updateList = from product in productStoreInfoList
                                     group product by new { product.ProductCode, product.StoreId } into g
                                     select new ArchStoreProductInfo
                                     {
                                         Deleted = g.FirstOrDefault().Deleted,
                                         Discontinued = g.FirstOrDefault().Discontinued,
                                         LastUpdated = lastUpdate,
                                         OnPromotion = g.FirstOrDefault().OnPromotion,
                                         ProductCode = g.FirstOrDefault().ProductCode,
                                         SellingPriceIncl = g.FirstOrDefault().SellingPriceIncl,
                                         SellingPriceInclPrice1 = g.FirstOrDefault().SellingPriceInclPrice1,
                                         SellingPriceInclPrice2 = g.FirstOrDefault().SellingPriceInclPrice2,
                                         SellingPriceInclPrice3 = g.FirstOrDefault().SellingPriceInclPrice3,
                                         SellingPriceInclPrice4 = g.FirstOrDefault().SellingPriceInclPrice4,
                                         SellingPriceInclPrice5 = g.FirstOrDefault().SellingPriceInclPrice5,
                                         POSItem = g.FirstOrDefault().POSItem,
                                         StoreId = RunningOnStoreId,
                                         TradeOnline = g.FirstOrDefault().TradeOnline,
                                         PromotionGroup = g.FirstOrDefault().PromotionGroup,
                                         AvailablePackQuantity = g.FirstOrDefault().AvailablePackQuantity,
                                         AvailableUnitQuantity = g.FirstOrDefault().AvailableUnitQuantity,
                                         StockOnHandField = g.FirstOrDefault().AvailablePackQuantity,
                                         StoreTypeId = StoreTypeId,
                                         IsInHolding = g.FirstOrDefault().IsInHolding,
                                         LimitedToStores = false
                                     };

                    _productStoreProductInfoRepository.Upsert(updateList, (p, s) => p.ProductCode == s.ProductCode && p.StoreId == RunningOnStoreId);

                    Debug($"Clearing cache");
                    foreach (var productStoreInfo in productStoreInfoList)
                    {
                        _staticCacheManager.Remove($"ArchStorePrice-{productStoreInfo.ProductCode}-{RunningOnStoreId}");
                    }

                    Debug($"Clean up temp data");
                    productStoreInfoList.Clear();
                }

                var sohCount = 0;
                if (response.StockOnHand != null && response.StockOnHand.Any())
                {
                    sohCount = response.StockOnHand.Length;
                    Debug($"Updating SOH for {response.StockOnHand.Length} items");
                    foreach (var stockItem in response.StockOnHand)
                    {
                        _archStoreProductInfoService.UpdateStockOnHand(stockItem.AvailableUnitQuantity, stockItem.AvailablePackQuantity, stockItem.AvailablePackQuantity, stockItem.ProductCode, RunningOnStoreId);
                    }
                }

                Debug($"SOH Update Completed.");
                skip += take;
                hasRecords = count > 0 || sohCount > 0;
            }
            while (hasRecords);
            SetLastUpdate(LastUpdateSettingParam);
        }

        private void EnsureDeletedPacks(ProductAvailabilityResponse.ProductAvailabilityElement baseProduct)
        {
            try
            {
                var getIncomingPacks = GetIncomingPacks(baseProduct.BaseCode).Result;
                var incomingPacks = getIncomingPacks.Select(p => new { BaseCode = p.BaseCode, ProductCode = p.ProductCode, IsPack = p.BaseCode == p.ProductCode }).ToList();

                var getAllPacks = GetAllPacks(baseProduct.BaseCode).Result;
                var allPacks = getAllPacks.Select(p => p.ProductCode).ToList();

                // Deadlock happens here
                var productPacks = GetProductPacks(allPacks).Result;

                var updatePacks = productPacks.Where(p => !incomingPacks.Any(x => x.ProductCode == p.ProductCode)).ToList();

                Debug($"Incoming pack count: {incomingPacks.Count()}. Existing pack count: {productPacks.Count}");
                UpdatePacks(updatePacks).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug($"EnsureDeletedPacks Exception: {ex}");
                Log("EnsureDeletedPacks Exception", ex);

                if (Debugger.IsAttached)
                {
                    throw;
                }
            }
        }

        private async System.Threading.Tasks.Task<List<ProductAvailabilityResponse.ProductAvailabilityElement>> GetIncomingPacks(string baseCode, int retry = 0)
        {
            try
            {
                if (retry < 3)
                {
                    return _productElementList.Where(p => p.BaseCode == baseCode).ToList();
                }
            }
            catch (Exception ex)
            {
                Log("GetIncomingPacks Exception", ex);

                await System.Threading.Tasks.Task.Delay(1000);
                return await GetIncomingPacks(baseCode, retry++);
            }

            return new List<ProductAvailabilityResponse.ProductAvailabilityElement>();
        }
        private async System.Threading.Tasks.Task<List<ArchProductDetail>> GetAllPacks(string baseCode, int retry = 0)
        {
            try
            {
                if (retry < 3)
                {
                    return _productDetailRepository.Table.Where(p => p.BaseCode == baseCode).AsSqlServer().WithReadUncommittedInScope().WithNoLockInScope().ToList();
                }
            }
            catch (Exception ex)
            {
                Log("GetAllPacks Exception", ex);

                await System.Threading.Tasks.Task.Delay(1000);
                return await GetAllPacks(baseCode, retry++);
            }

            return new List<ArchProductDetail>();
        }
        private async System.Threading.Tasks.Task<List<ArchStoreProductInfo>> GetProductPacks(List<string> allPacks, int retry = 0)
        {
            try
            {
                if (retry < 3)
                {
                    return _productStoreProductInfoRepository.Table.Where(p => p.StoreId == RunningOnStoreId && allPacks.Any(a => a == p.ProductCode))
                        .AsSqlServer().WithReadUncommittedInScope().WithNoLockInScope().ToList();
                }
            }
            catch (Exception ex)
            {
                Log("GetProductPacks Exception", ex);

                await System.Threading.Tasks.Task.Delay(1000);
                return await GetProductPacks(allPacks, retry++);
            }

            return new List<ArchStoreProductInfo>();
        }
        private async System.Threading.Tasks.Task UpdatePacks(List<ArchStoreProductInfo> updatePacks, int retry = 0)
        {
            try
            {
                var remainingPacks = new List<ArchStoreProductInfo>();
                remainingPacks.AddRange(updatePacks);

                if (retry < 3)
                {
                    foreach (var packItem in updatePacks)
                    {
                        Debug($"Marking Pack as Deleted: {packItem.ProductCode}");

                        _productStoreProductInfoRepository.Table
                        .Where(p => p.ProductCode == packItem.ProductCode && p.StoreId == RunningOnStoreId)
                        .Set(p => p.Deleted, true)
                        .Set(p => p.Discontinued, true)
                        .Set(p => p.IsInHolding, true).Update();

                        remainingPacks.Remove(packItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("UpdatePacks Exception", ex);

                await System.Threading.Tasks.Task.Delay(1000);
                await UpdatePacks(updatePacks, retry++);
            }
        }

        protected override void Consume(ProductAvailabilityResponse.ProductAvailabilityElement item)
        {
            if (item.BaseCode == item.ProductCode)
            {
                EnsureDeletedPacks(item);

                if (_productElementList.Any(p => p.BaseCode == item.BaseCode))
                    _productElementList.RemoveAll(p => p.BaseCode == item.BaseCode);
            }
            Debug($"Completed processing {item.ProductCode}");
        }

        protected override void CollectData() { }
        protected override void OnCompleting()
        {
            base.OnCompleting();
            try
            {
                _dataProvider.ExecuteNonQuery("LoadProductDisplay", new LinqToDB.Data.DataParameter("StoreId", RunningOnStoreId));
            }
            catch (Exception ex)
            {
                //this.Error(ex.Message, ex);
            }
        }
    }

}
