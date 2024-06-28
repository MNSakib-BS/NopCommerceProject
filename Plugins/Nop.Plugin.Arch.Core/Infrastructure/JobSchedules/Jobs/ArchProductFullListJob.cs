using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArchServiceReference;
using FluentValidation.Results;
using Hangfire.Storage;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using Arch.Core.Domain.StoreType;
using Arch.Core.Infrastructure.JobSchedules;
using Arch.Core.Services.Catalog;
using Arch.Core.Services.Helpers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using static LinqToDB.Reflection.Methods.LinqToDB;
using ILogger = Nop.Services.Logging.ILogger;


namespace Arch.Core.Infrastructure.JobSchedules.Jobs
{
    /// <summary>
    /// Represents a task for calling the arch api and resolving the product full list
    /// </summary>
    public partial class ArchProductFullListJob : ArchScheduledJobBase<ArchProductFullListJob.ArchProductDetailModel>
    {
        protected override Type TaskType => typeof(ArchProductFullListJob);
        public override bool RunOnlyOnGlobalStore => true;
        private const string LastUpdateSettingParam = "ArchProductFullListTask_LastUpdate";
        private const string BatchSizeSettingParam = "ArchProductFullList_BatchSize";

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<ArchProductDetail> _productDetailRepository;
        private readonly IRepository<ArchStoreProductInfo> _productStoreProductInfoRepository;
        private readonly IArchStoreProductInfoService _archStoreProductInfoService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<StoreTypeMapping> _storeTypeMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ArchStoreProductInfo> _archStoreProductInfoRepository;

        private SpecificationAttribute _onPromotionSpecificationAttribute;
        private ProductAttribute _packSizeProductAttribute;
        private ProductAttribute _weightProductAttribute;
        private IList<PredefinedProductAttributeValue> _weightPredefinedProductAttributeValues;

        private HashSet<ProductDataResponse.ProductDataElement> _productElementList;

        public ArchProductFullListJob(IProductService productService,
            ICategoryService categoryService,
            ISettingService settingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            ISpecificationAttributeService specificationAttributeService,
            ILogger logger,
            IObjectConverter objectConverter,
            IRepository<ArchProductDetail> productDetailRepository,
            IRepository<ArchStoreProductInfo> productStoreProductInfoRepository,
            IArchStoreProductInfoService archStoreProductInfoService,
            IStaticCacheManager staticCacheManager,
            ILogger<ScheduledJobBase<object>> jobLogger,
            IRepository<StoreTypeMapping> storeTypeMappingRepository,
            IRepository<ArchStoreProductInfo> archStoreProductInfoRepository)
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
            _productAttributeService = productAttributeService;
            _specificationAttributeService = specificationAttributeService;
            _categoryService = categoryService;
            _weightPredefinedProductAttributeValues = new List<PredefinedProductAttributeValue>();
            _productDetailRepository = productDetailRepository;
            _productStoreProductInfoRepository = productStoreProductInfoRepository;
            _archStoreProductInfoService = archStoreProductInfoService;
            _staticCacheManager = staticCacheManager;
            _storeTypeMappingRepository = storeTypeMappingRepository;
            _productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
            _archStoreProductInfoRepository = archStoreProductInfoRepository;
            _productElementList = new HashSet<ProductDataResponse.ProductDataElement>();
        }

        #region Methods

        protected override void OnExecuting()
        {
            base.OnExecuting();

            Debug($"Executing {nameof(ArchProductFullListJob)} for StoreID: {RunningOnStoreId}");
            //fetch the on promotion specification attribute
            _onPromotionSpecificationAttribute = _specificationAttributeService.GetSpecificationAttributes(storeId: 0).FirstOrDefault(i => i.Name == "On Promotion");

            // fetch the product attribute used for pack size options
            _packSizeProductAttribute = _productAttributeService.GetAllProductAttributes(storeId: 0).FirstOrDefault(i => i.Name == "Pack Size");

            // fetch the product attribute and values used for variable weight items
            _weightProductAttribute = _productAttributeService.GetAllProductAttributes(storeId: 0).FirstOrDefault(i => i.Name == "Weight");
            if (_weightProductAttribute != null)
                _weightPredefinedProductAttributeValues = _productAttributeService.GetPredefinedProductAttributeValues(_weightProductAttribute.Id);
        }

        protected override void CollectData()
        {
            Log("Collecting Data", null, LogLevel.Debug);
            var lastUpdate = GetLastUpdate(LastUpdateSettingParam);
            var take = GetSetting(BatchSizeSettingParam, 1000);
            var skip = 0;
            bool hasRecords;
            List<ArchProductDetail> productDetailList = new List<ArchProductDetail>();
            var productList = new HashSet<Product>();
            var isGlobalStore = _storeService.GetStoreById(RunningOnStoreId).IsGlobalStore;

            if (!isGlobalStore) //If not the global store, do not process product information.
                return;

            do
            {
                Debug($"Calling ArchAPI / take:{take} / skip:{skip}");
                var response = ArchClient.GetProductDataAsync(new ProductDataRequest
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

                var data = response.List;

                var count = data.Length;
                Debug($"Producing {count} items");


                foreach (var item in data)
                {
                    _productElementList.Add(item);

                    var productDetail = new ArchProductDetail
                    {
                        ProductCode = item.ProductCode,
                        PriceActivation = item.PriceActivation,
                        BaseCode = item.BaseCode,
                        BaseDescription = item.BaseDescription,
                        BrandName = item.BrandName,
                        DepartmentName = item.DepartmentName,
                        DepartmentNumber = item.DepartmentNumber,
                        Depth = item.Depth,
                        Height = item.Height,
                        HouseBrand = item.HouseBrand,
                        IsMasterProduct = item.IsMasterProduct,
                        KeyLine = item.KeyLine,
                        KVI = item.KVI,
                        LastUpdated = lastUpdate,
                        Mass = item.Mass,
                        MinStockQty = item.MinStockQty,
                        NormalPriceIncl = item.NormalPriceIncl,
                        PackDescription = item.PackDescription,
                        PackSize = item.PackSize,
                        PackUnitSize = item.PackUnitSize,
                        PriceDeactivation = item.PriceDeactivation,
                        PriceLinkCode = item.PriceLinkCode,
                        Taxable = item.Taxable,
                        TaxRate = item.TaxRate,
                        UnitOfMeassure = item.UnitOfMeassure,
                        UnitSize = item.UnitSize,
                        WeightType = item.WeightType,
                        Width = item.Width,
                        StoreTypeId = StoreTypeId
                    };

                    if (isGlobalStore)
                    {
                        if (!productDetailList.Any(p => p.ProductCode == productDetail.ProductCode && p.BaseCode == productDetail.BaseCode && p.StoreTypeId == productDetail.StoreTypeId))
                            productDetailList.Add(productDetail);

                        var productDetailModel = GetModelFromProductDetail(productDetail, lastUpdate);
                        var product = GetProductFromDetailModel(productDetailModel);

                        if (productDetailModel != null && !productList.Any(p => p.ProductCodeField == productDetail.ProductCode && p.BaseCodeField == productDetail.BaseCode && p.StoreTypeId == productDetail.StoreTypeId))
                            productList.Add(product);
                    }
                }

                if (productDetailList.Any())
                {
                    Debug($"Merge Product Details");
                    _productDetailRepository.Upsert(productDetailList, (p, s) => p.ProductCode == s.ProductCode && p.BaseCode == s.BaseCode && p.StoreTypeId == s.StoreTypeId);
                }

                // Ensure all updated products have been added/updated to the Product table to ensure Pack Size Attributes are correctly set
                if (productList.Any())
                {
                    _productRepository.Upsert(productList, (d, s) => s.StoreTypeId == d.StoreTypeId && d.ProductCodeField == s.ProductCodeField && d.BaseCodeField == s.BaseCodeField);
                }

                productDetailList.Clear();
                productList.Clear();

                skip += take;
                hasRecords = count > 0;
            }
            while (hasRecords);
        }

        protected override void Produce()
        {
            var lastUpdate = GetLastUpdate(LastUpdateSettingParam);

            var query = from product in _productDetailRepository.Table.Where(p => p.StoreTypeId == StoreTypeId && p.LastUpdated >= lastUpdate)
                        select new
                        {
                            ProductCode = product.ProductCode,
                            PriceActivation = product.PriceActivation,
                            BaseCode = product.BaseCode,
                            BaseDescription = product.BaseDescription,
                            BrandName = product.BrandName,
                            Deleted = false,
                            DepartmentName = product.DepartmentName,
                            DepartmentNumber = product.DepartmentNumber,
                            Depth = product.Depth,
                            Discontinued = false,
                            FullDescription = product.FullDescription,
                            Height = product.Height,
                            HouseBrand = product.HouseBrand,
                            IsMasterProduct = product.IsMasterProduct,
                            KeyLine = product.KeyLine,
                            KVI = product.KVI,
                            LastUpdated = lastUpdate,
                            Mass = product.Mass,
                            MinStockQty = product.MinStockQty,
                            NormalPriceIncl = product.NormalPriceIncl,
                            OnPromotion = false,
                            PackDescription = product.PackDescription,
                            PackSize = product.PackSize,
                            PackUnitSize = product.PackUnitSize,
                            POSItem = true,
                            PriceDeactivation = product.PriceDeactivation,
                            PriceLinkCode = product.PriceLinkCode,
                            Taxable = product.Taxable,
                            TaxRate = product.TaxRate,
                            TradeOnline = true,
                            UnitOfMeassure = product.UnitOfMeassure,
                            UnitSize = product.UnitSize,
                            WeightType = product.WeightType,
                            Width = product.Width,
                            ProductDetailId = product.Id,
                            StoreTypeId = product.StoreTypeId
                        };

            var records = (from product in query
                           select new ArchProductDetailModel()
                           {
                               ProductCode = product.ProductCode,
                               PriceActivation = product.PriceActivation,
                               BaseCode = product.BaseCode,
                               BaseDescription = product.BaseDescription,
                               BrandName = product.BrandName,
                               Deleted = product.Deleted,
                               DepartmentName = product.DepartmentName,
                               DepartmentNumber = product.DepartmentNumber,
                               Depth = product.Depth,
                               Discontinued = product.Discontinued,
                               FullDescription = product.FullDescription,
                               Height = product.Height,
                               HouseBrand = product.HouseBrand,
                               IsMasterProduct = product.IsMasterProduct,
                               KeyLine = product.KeyLine,
                               KVI = product.KVI,
                               LastUpdated = lastUpdate,
                               Mass = product.Mass,
                               MinStockQty = product.MinStockQty,
                               NormalPriceIncl = product.NormalPriceIncl,
                               OnPromotion = product.OnPromotion,
                               PackDescription = product.PackDescription,
                               PackSize = product.PackSize,
                               PackUnitSize = product.PackUnitSize,
                               POSItem = product.POSItem,
                               PriceDeactivation = product.PriceDeactivation,
                               PriceLinkCode = product.PriceLinkCode,
                               Taxable = product.Taxable,
                               TaxRate = product.TaxRate,
                               TradeOnline = product.TradeOnline,
                               UnitOfMeassure = product.UnitOfMeassure,
                               UnitSize = product.UnitSize,
                               WeightType = product.WeightType,
                               Width = product.Width,
                               ProductDetaildId = product.ProductDetailId,
                               StoreDetailId = RunningOnStoreId,
                               StoreTypeId = product.StoreTypeId
                           }).AsSqlServer().WithReadUncommittedInScope().ToList();

            Debug($"Enqueuing {records.Count} products");

            records.ForEach(p => EnqueueItem(p));
        }

        private ArchProductDetailModel GetModelFromProductDetail(ArchProductDetail product, DateTime lastUpdate)
        {
            var detail = new ArchProductDetailModel
            {
                ProductCode = product.ProductCode,
                PriceActivation = product.PriceActivation,
                BaseCode = product.BaseCode,
                BaseDescription = product.BaseDescription,
                BrandName = product.BrandName,
                DepartmentName = product.DepartmentName,
                DepartmentNumber = product.DepartmentNumber,
                Depth = product.Depth,
                FullDescription = product.FullDescription,
                Height = product.Height,
                HouseBrand = product.HouseBrand,
                IsMasterProduct = product.IsMasterProduct,
                KeyLine = product.KeyLine,
                KVI = product.KVI,
                LastUpdated = lastUpdate,
                Mass = product.Mass,
                MinStockQty = product.MinStockQty,
                NormalPriceIncl = product.NormalPriceIncl,
                PackDescription = product.PackDescription,
                PackSize = product.PackSize,
                PackUnitSize = product.PackUnitSize,
                PriceDeactivation = product.PriceDeactivation,
                PriceLinkCode = product.PriceLinkCode,
                Taxable = product.Taxable,
                TaxRate = product.TaxRate,
                UnitOfMeassure = product.UnitOfMeassure,
                UnitSize = product.UnitSize,
                WeightType = product.WeightType,
                Width = product.Width,
                StoreTypeId = product.StoreTypeId,
            };
            return detail;
        }

        protected override void OnCompleting()
        {
            var isGlobalStore = _storeService.GetStoreById(RunningOnStoreId).IsGlobalStore;

            if (!isGlobalStore) //If not the global store, do not process product information.
                return;

            Debug($"Deleting tombstoned products for processed batches...");
            _productService.DeleteTombstonedProducts();

            SetLastUpdate(LastUpdateSettingParam);

            base.OnCompleting();
        }

        protected override void Consume(ArchProductDetailModel item)
        {
            var productName = GenerateProductName(item);
            Debug($"Consuming {productName}");

            var product = GetProduct(item);

            if (item.Deleted && product.Deleted)
                return;

            if (!_archSettings.DisableCategoryImport)
            {
                Debug($"Linking {productName} to Category number {item.DepartmentNumber}");
                _productService.EnsureProductDepartment(product, item.DepartmentNumber, SaveStoreMappings, StoreTypeId, RunningOnStoreId);
            }

            _productService.AddProductBrand(product, item.BrandName, SaveStoreMappings, SaveStoreMappings, SaveManufacturerSlug);

            if (_weightProductAttribute != null)
                _productService.EnsureWeightProductAttributes(product, item.WeightType, _weightProductAttribute.Id, _weightPredefinedProductAttributeValues);

            EnsurePackSizes(item, product);

            if (_onPromotionSpecificationAttribute != null)
                _productService.EnsureOnPromotionProductAttribute(product, _onPromotionSpecificationAttribute.Id);

            // Ensure product is not marked as deleted
            product.Deleted = false;
            product.LimitedToStores = false;
            _productService.UpdateProduct(product);

            SaveProductSlug(product);

            SaveStoreMappings(product, () => { _productService.UpdateProduct(product); }, false, false);

            if (!item.IsMasterProduct)
            {
                _productService.UpdateProductStoreMappings(product, new List<int> { RunningOnStoreId });
            }

            Debug($"Completed processing {item.FullDescription}");
        }

        protected override void SaveStoreMappings<TEntity>(TEntity entity, Action updateEntityAction)
        {
            base.SaveStoreMappings(entity, updateEntityAction, false, false);
        }

        private void EnsurePackSizes(ArchProductDetailModel item, Product product)
        {
            if (_packSizeProductAttribute != null)
            {
                try
                {
                    _productService.EnsurePackSizeProductAttributes(product, item.BaseCode, item.ProductCode, item.PackSize, _packSizeProductAttribute.Id, RunningOnStoreId, this.StoreTypeId);
                }
                catch (Exception ex)
                {
                    Log("Pack Size attribute error", ex, LogLevel.Error);
                }
            }
        }

        private void UpdateArchStoreProductDetails(ArchProductDetailModel item)
        {
            var lastUpdated = DateTime.Now;
            _productDetailRepository.Table.Where(p => p.Id == item.ProductDetaildId).Set(s => s.LastUpdated, lastUpdated).Update();

            if (item.StoreDetailId != 0)
            {
                _archStoreProductInfoRepository.Table.Where(p => p.ProductCode == item.ProductCode && p.StoreId == RunningOnStoreId).Set(s => s.LastUpdated, lastUpdated).Update();
            }
        }

        private Product GetProductFromDetailModel(ArchProductDetailModel item)
        {
            var product = new Product();
            Map(item, product);

            product.ProductType = ProductType.SimpleProduct;
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.VisibleIndividually = true;
            product.ParentGroupedProductId = 0;
            product.ProductTemplateId = 1;
            product.IsShipEnabled = true;
            product.TaxCategoryId = 0;
            product.LowStockActivityId = 0;
            product.NotifyAdminForQuantityBelow = 1;
            product.OrderMinimumQuantity = 1;
            if (product.OrderMaximumQuantity == 0)
                product.OrderMaximumQuantity = FetchOrderMaximumQuantity(product);
            product.StoreTypeId = StoreTypeId;
            product.LimitedToStores = false;
            product.Published = true;
            return product;
        }

        private Product GetProduct(ArchProductDetailModel item)
        {
            var product = _productService.GetProductByProductCode(item.ProductCode, storeId: RunningOnStoreId, storeTypeId: StoreTypeId);
            if (product == null)
            {
                product = GetProductFromDetailModel(item);
                _productService.InsertProduct(product);
            }
            return product;
        }

        private int FetchOrderMaximumQuantity(Product product)
        {
            var maxQuantity = _archSettings.OrderMaximumQuantityDefault;
            return maxQuantity * (int)(product.PackSizeField.HasValue ? product.PackSizeField.Value : 1);
        }

        private void Map(ArchProductDetailModel source, Product destination)
        {
            // items the are available from GetProductFullListAsync but not from GetProductListAsync
            destination.AvailablePackQuantityField = source.AvailablePackQuantity;
            destination.AvailableUnitQuantityField = source.AvailableUnitQuantity;
            destination.UnitOfMeassureField = source.UnitOfMeassure;
            destination.UnitSizeField = Convert.ToDecimal(source.UnitSize);
            destination.PackSizeField = source.PackSize;

            // items the are available from both GetProductListAsync and GetProductFullListAsync
            destination.BaseCodeField = source.BaseCode;
            destination.ProductCodeField = source.ProductCode;
            destination.PackDescriptionField = source.PackDescription;
            destination.PriceLinkCodeField = source.PriceLinkCode;
            destination.BaseDescriptionField = source.BaseDescription;
            destination.DepartmentNumberField = source.DepartmentNumber;
            destination.BrandNameField = source.BrandName;
            destination.DiscontinuedField = source.Discontinued;
            destination.HouseBrandField = source.HouseBrand;
            destination.KVIField = source.KVI;
            destination.KeyLineField = source.KeyLine;
            destination.NormalPriceInclField = source.NormalPriceIncl;
            destination.OnPromotionField = false;
            destination.POSItemField = Convert.ToBoolean(source.POSItem);
            destination.PackUnitSizeField = source.PackUnitSize;
            destination.PriceActivationField = source.PriceActivation;
            destination.PriceDeactivationField = source.PriceDeactivation;
            destination.PromotionGroupField = source.PromotionGroup;
            destination.SellingPriceInclField = source.SellingPriceIncl;
            destination.SellingPriceInclPrice1Field = source.SellingPriceInclPrice1;
            destination.SellingPriceInclPrice2Field = source.SellingPriceInclPrice2;
            destination.SellingPriceInclPrice3Field = source.SellingPriceInclPrice3;
            destination.SellingPriceInclPrice4Field = source.SellingPriceInclPrice4;
            destination.SellingPriceInclPrice5Field = source.SellingPriceInclPrice5;
            destination.TaxRateField = source.TaxRate;
            destination.TradeOnlineField = source.TradeOnline;
            destination.WidthField = source.Width;
            destination.DepthField = source.Depth;
            destination.HeightField = source.Height;
            destination.MassField = source.Mass;
            destination.WeightTypeField = source.WeightType;
            destination.IsTaxExempt = !Convert.ToBoolean(source.Taxable);
            destination.MinStockQuantity = source.MinStockQty;
            destination.ShortDescription = source.BaseDescription;
            destination.Deleted = false;
            destination.Width = source.Width;
            destination.Length = source.Depth;
            destination.Height = source.Height;
            destination.Weight = source.Mass;
            destination.Name = GenerateProductName(source);

            // Include Department name to improve search results
            if (string.IsNullOrEmpty(destination.FullDescription))
            {
                destination.FullDescription = $"{destination.Name} {source.DepartmentName}";
            }

            destination.Sku = source.ProductCode;
            destination.Price = source.SellingPriceIncl;
            destination.UpdatedOnUtc = DateTime.UtcNow;
            destination.StockQuantity = Convert.ToInt32(source.AvailablePackQuantity);
            destination.IsTombstoned = false;
            destination.Published = true;

            destination.ManageInventoryMethod = _archSettings.TrackInventory ? ManageInventoryMethod.ManageStock : ManageInventoryMethod.DontManageStock;
            destination.DisplayStockAvailability = _archSettings.DisplayStockAvailability;
            destination.DisplayStockQuantity = _archSettings.DisplayStockQuantity;
        }

        private string GenerateProductName(ArchProductDetailModel item)
        {
            var name = string.Empty;
            if (!string.IsNullOrEmpty(item.BaseDescription))
                name = item.BaseDescription.Trim();

            if (!string.IsNullOrEmpty(item.BrandName) && !name.StartsWith(item.BrandName.Trim()))
                name = $"{item.BrandName.Trim()} {name}";

            if (item.UnitSize > 1)
            {
                name = $"{name} {item.UnitSize}{item.UnitOfMeassure:n0}";
            }
            else
            {
                name = $"{name} {item.UnitOfMeassure:n0}";
            }

            if (item.PackSize > 1)
            {
                name = $"{name.Trim()} x {Convert.ToInt32(item.PackSize)}";
            }

            return name.Trim();
        }
        #endregion

        #region DataModel
        public partial class ArchProductDetailModel
        {
            #region "Properties"
            public decimal AvailablePackQuantity { get; set; }

            public decimal AvailableUnitQuantity { get; set; }

            public string BaseCode { get; set; }

            public string BaseDescription { get; set; }

            public string BrandName { get; set; }

            public bool Deleted { get; set; }

            public string DepartmentName { get; set; }

            public int DepartmentNumber { get; set; }

            public decimal Depth { get; set; }

            public bool Discontinued { get; set; }

            public string FullDescription { get; set; }

            public decimal Height { get; set; }

            public bool HouseBrand { get; set; }

            public bool IsMasterProduct { get; set; }

            public bool KVI { get; set; }

            public bool KeyLine { get; set; }

            public decimal Mass { get; set; }

            public int MinStockQty { get; set; }

            public decimal NormalPriceIncl { get; set; }

            public bool OnPromotion { get; set; }

            public bool POSItem { get; set; }

            public string PackDescription { get; set; }

            public decimal PackSize { get; set; }

            public string PackUnitSize { get; set; }

            public DateTime PriceActivation { get; set; }

            public DateTime PriceDeactivation { get; set; }

            public string PriceLinkCode { get; set; }

            public string ProductCode { get; set; }

            public int PromotionGroup { get; set; }

            public decimal SellingPriceIncl { get; set; }

            public decimal SellingPriceInclPrice1 { get; set; }

            public decimal SellingPriceInclPrice2 { get; set; }

            public decimal SellingPriceInclPrice3 { get; set; }

            public decimal SellingPriceInclPrice4 { get; set; }

            public decimal SellingPriceInclPrice5 { get; set; }

            public decimal TaxRate { get; set; }

            public bool Taxable { get; set; }

            public bool TradeOnline { get; set; }

            public string UnitOfMeassure { get; set; }

            public float UnitSize { get; set; }

            public int WeightType { get; set; }

            public decimal Width { get; set; }

            public DateTime LastUpdated { get; set; }
            public int ProductDetaildId { get; internal set; }
            public int StoreDetailId { get; internal set; }
            public int StoreTypeId { get; set; }
            #endregion
        }
        #endregion
    }
}
