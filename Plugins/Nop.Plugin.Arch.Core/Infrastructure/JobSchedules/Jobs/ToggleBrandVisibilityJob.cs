using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nop.Arch.Services.Helpers;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Stores;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    public partial class ToggleBrandVisibilityJob : ScheduledJobBase<Manufacturer>
    {
        protected override Type TaskType => typeof(ToggleBrandVisibilityJob);

        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;

        public ToggleBrandVisibilityJob(ISettingService settingService,
            IManufacturerService manufacturerService,
            IStoreContext storeContext,
            IStoreService storeService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
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
            _manufacturerService = manufacturerService;
            _productService = productService;
        }

        protected override void Produce()
        {
            var manufacturers = _manufacturerService.GetAllManufacturers(string.Empty, showHidden: true, storeId: RunningOnStoreId);

            Debug($"Producing {manufacturers.Count} brands");
            foreach (var manufacturer in manufacturers)
            {
                EnqueueItem(manufacturer);
            }
            Debug($"Completed producing");
        }

        protected override void Consume(Manufacturer item)
        {
            Debug($"Checking {item.Name} usage");

            //var productManufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(item.Id);
            //var products = _productService.GetProductsByIds(productManufacturers.Select(i => i.ProductId).ToArray());

            //var totalPublishedNotInHolding = products.Count(i => i.Published);
            //var isManufacturerUsed = totalPublishedNotInHolding > 0;

            //var isPublishedOld = item.Published;
            //item.Published = isManufacturerUsed;

            //var updateRequired = isPublishedOld != item.Published;
            //if (updateRequired)
            //{
            //    _manufacturerService.UpdateManufacturer(item);
            //}

            Debug($"Completed processing {item.Name}");
        }

        protected override void CollectData() { }
    }
}
