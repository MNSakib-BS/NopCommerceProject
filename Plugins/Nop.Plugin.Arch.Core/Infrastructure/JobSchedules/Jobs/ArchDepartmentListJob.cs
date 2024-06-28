using System;
using ArchServiceReference;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using ILogger = Nop.Services.Logging.ILogger;
using Nop.Data;
using Arch.Core.Domain.StoreType;
using System.Diagnostics;
using Arch.Core.Services.Helpers;

namespace Arch.Core.Infrastructure.JobSchedules.Jobs
{
    /// <summary>
    /// Represents a task for calling the arch api and resolving the debtors list
    /// </summary>
    public class ArchDepartmentListJob : ArchScheduledJobBase<GetDepartmentResponse.DepartmentElement>
    {
        protected override Type TaskType => typeof(ArchDepartmentListJob);

        private const string LastUpdateSettingParam = "ArchDepartmentListTask_LastUpdate";
        private readonly CategoryType _categoryType = CategoryType.Department;
        private readonly ICategoryService _categoryService;

        public ArchDepartmentListJob(
            ICategoryService categoryService,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IStoreContext storeContext,
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
            _categoryService = categoryService;
        }

        protected override void Produce()
        {
            if (_archSettings.DisableCategoryImport)
            {
                return;
            }

            if (_archSettings.UseGlobalStoreForCategories)// Only process Departments from one Store, AE is responsible for Departments/Categories in this case
            {
                var isGlobalStore = _storeService.GetStoreById(RunningOnStoreId).IsGlobalStore;
                if (!isGlobalStore) { return; }
            }
            var categories = _categoryService.GetAllCategories(RunningOnStoreId, true);

            // if we do not have any categories, get the complete list
            var lastUpdate = categories == null || !categories.Any() ? new DateTime(2020, 1, 1) : GetLastUpdate(LastUpdateSettingParam);

            Debug($"Calling ArchAPI");
            var response = ArchClient.GetDepartmentListAsync(new GetDepartmentRequest
            {
                LastUpdate = lastUpdate,
                SystemAuthenticationCode = _archSettings.SystemAuthenticationCode
            });

            if (!response.Result.Success)
            {
                Debug(response.Result.ResponseMessage);
                return;
            }

            var count = response.Result.List.Length;
            Debug($"Producing {count} items");
            for (var i = 0; i < count; i++)
            {
                var item = response.Result.List[i];
                EnqueueItem(item);
            }
            Debug($"Completed Producing");

            SetLastUpdate(LastUpdateSettingParam);
        }

        protected override void Consume(GetDepartmentResponse.DepartmentElement item)
        {
            Log($"Consuming {item.DepartmentName}");

            var category = _categoryService.GetCategoryByExternalId(item.DepartmentNumber, CategoryType.Department, item.ReportingDepartmentNumber, storeTypeId: StoreTypeId);

            AddOrUpdateEntity(category, item);

            Log($"Completed processing {item.DepartmentName}");
        }

        private int AddOrUpdateEntity(Category category, GetDepartmentResponse.DepartmentElement item)
        {
            var categoryName = item.DepartmentName;
            if (category != null)
            {
                Log($"Category exists {item.DepartmentName}");
                if (_archSettings.AutoSyncEnabled)
                {
                    Map(item, category, true);

                    _categoryService.UpdateCategory(category);
                }
            }
            else
            {
                Log($"Adding Category {item.DepartmentName}");
                category = new Category
                {
                    ExternalId = item.DepartmentNumber,
                    CategoryType = CategoryType.Department,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreTypeId = StoreTypeId,
                    ArchManaged = true,
                    LimitedToStores = false,
                };

                Map(item, category);

                _categoryService.InsertCategory(category);
            }

            if(category.Name != categoryName)
            {
                ClearCategorySlug(category, categoryName);
            }

            //search engine name
            SaveCategorySlug(category);

            SaveStoreMappings(category, () => { _categoryService.UpdateCategory(category); }, limitedToStore: false);

            return category.Id;
        }

        private void ClearCategorySlug(Category category, string categoryName)
        {
           var seName= _urlRecordService.ValidateSeName(category, categoryName, category.Name, true);
                _urlRecordService.SaveSlug(category, seName, 0);
        }

        private void Map(GetDepartmentResponse.DepartmentElement source, Category destination, bool existingCategory = false)
        {
            var excludeList = new[] { "delete", "unbound" };
            var autoPublish = !excludeList.Contains(source.DepartmentName.ToLower());

            destination.Name = source.DepartmentName;
            destination.ExternalParentId = source.ReportingDepartmentNumber;
            destination.Published = autoPublish;
            destination.IncludeInTopMenu = autoPublish && !existingCategory;
            destination.PageSize = 20;
            destination.CategoryTemplateId = 1;
            destination.UpdatedOnUtc = DateTime.UtcNow;
            destination.StoreTypeId = StoreTypeId;
            destination.LimitedToStores = false;
        }

        protected override void OnCompleting()
        {
            if (!_archSettings.DisableCategoryImport)
            {
                if (_archSettings.UseGlobalStoreForCategories)// Only process Departments from one Store, AE is responsible for Departments/Categories in this case
                {
                    var isGlobalStore = _storeService.GetStoreById(RunningOnStoreId).IsGlobalStore;
                    if (!isGlobalStore) { return; }
                }
                // Ensure all categories are mapped once we have processed them. A seperate scheduler for this can cause issues since they can run concurrently
                var categories = _categoryService.GetAllCategories(RunningOnStoreId, true);
                foreach (var item in categories)
                {
                    
                    if (item.ExternalId != item.ExternalParentId)
                    {

                        var foundParent = _categoryService.GetCategoryByExternalId(item.ExternalParentId, _categoryType, storeId: RunningOnStoreId, storeTypeId: this.StoreTypeId);
                        item.ParentCategoryId = foundParent?.Id ?? 0;
                        _categoryService.UpdateCategory(item);
                    }
                }
            }
            base.OnCompleting();
        }

        protected override void CollectData() { }
    }
}