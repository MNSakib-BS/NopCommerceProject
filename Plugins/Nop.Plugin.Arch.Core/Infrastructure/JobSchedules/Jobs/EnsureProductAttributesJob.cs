using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nop.Arch.Domain.StoreType;
using Nop.Arch.Services.Catalog;
using Nop.Arch.Services.Helpers;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Seo;
using Nop.Services.Stores;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    public partial class EnsureProductAttributesJob : ScheduledJobBase<Product>
    {
        protected override Type TaskType => typeof(EnsureProductAttributesJob);

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IArchStoreProductInfoService _archStoreProductInfoService;
        private readonly IRepository<Product> _productRepository;

        private ProductAttribute _packSizeProductAttribute;

        private ProductAttribute _weightProductAttribute;
        private IList<PredefinedProductAttributeValue> _weightPredefinedProductAttributeValues;

        private SpecificationAttribute _onPromotionSpecificationAttribute;

        public EnsureProductAttributesJob(ISettingService settingService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IStoreContext storeContext,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
            ISpecificationAttributeService specificationAttributeService,
            IArchStoreProductInfoService archStoreProductInfoService,
            ILogger<ScheduledJobBase<object>> jobLogger)
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

            _weightPredefinedProductAttributeValues = new List<PredefinedProductAttributeValue>();
            _archStoreProductInfoService = archStoreProductInfoService;
            _productRepository = EngineContext.Current.Resolve<IRepository<Product>>();

        }

        protected override void Produce()
        {
            // fetch the product attribute used for pack size options
            _packSizeProductAttribute = _productAttributeService
                .GetAllProductAttributes(storeId: 0).FirstOrDefault(i => i.Name == "Pack Size");

            // fetch the product attribute and values used for variable weight items
            _weightProductAttribute = _productAttributeService.GetAllProductAttributes(storeId: 0)
                .FirstOrDefault(i => i.Name == "Weight");

            if (_weightProductAttribute != null)
            {
                _weightPredefinedProductAttributeValues = _productAttributeService
                    .GetPredefinedProductAttributeValues(_weightProductAttribute.Id);
            }

            //fetch the on promotion specification attribute
            _onPromotionSpecificationAttribute = _specificationAttributeService
                .GetSpecificationAttributes(storeId: 0).FirstOrDefault(i => i.Name == "On Promotion");

            var products = FetchProducts();

            Log($"Producing {products.Count} products");

            foreach (var product in products)
                EnqueueItem(product);

            Debug($"Completed producing");
        }

        private List<Product> FetchProducts()
        {
            var productsQuery = _productRepository.Table;
            productsQuery = productsQuery.Where(p => p.StoreTypeId == StoreTypeId);
            productsQuery = _archStoreProductInfoService.FilterProducts(productsQuery, RunningOnStoreId);
            var notInHolding = productsQuery.ToList();

            var products = new List<Product>();

            products.AddRange(notInHolding);

            return products;
        }

        protected override void Consume(Product item)
        {
            Debug($"Processing {item.Name}");

            // NOTE: the logic here differs from the ProductFullListTask as this task is intended to test the positive case only
            // i.e. not used to remove the attributes from the products, but to ensure they are associated when the products have properties indicating they need the attributes assigned
            // the removal of the attributes should take place when the ProductFullListTask runs.

            // This task is also only intended to be run when setting up up a new system, and is just to ensure that if the arch specific tasks run out of a certain order
            // we can run this and ensure the attributes and departments are associated correctly.

            // updates could come from the api or the estore, as the estore can add/update/delete weight values, that would need to be associated to the products
            // the ProductAttributeService makes a call to EnsureWeightProductAttributes when predefined attributes are updated/inserted/deleted
            var processVariableWeightProduct = _weightProductAttribute != null && item.WeightTypeField == (int)WeightType.PerWeight;
            if (processVariableWeightProduct)
                _productService.EnsureWeightProductAttributes(item, item.WeightTypeField, _weightProductAttribute.Id, _weightPredefinedProductAttributeValues);

            // updates should only be comming through the api for this property
            var processPackSizeProduct = _packSizeProductAttribute != null && item.PackSizeField.HasValue;
            if (processPackSizeProduct)
                _productService.EnsurePackSizeProductAttributes(item, item.BaseCodeField, item.ProductCodeField, item.PackSizeField, _packSizeProductAttribute.Id, RunningOnStoreId, StoreTypeId);

            Debug($"Completed processing {item.Name}");
        }

        protected override void CollectData() { }
    }
}
