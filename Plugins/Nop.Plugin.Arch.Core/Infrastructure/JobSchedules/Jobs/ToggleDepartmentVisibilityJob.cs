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
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    /// <summary>
    /// Represents a task for updating department category visibility
    /// </summary>
    public partial class ToggleDepartmentVisibilityJob: ScheduledJobBase<Category>
    {
        protected override Type TaskType => typeof(ToggleDepartmentVisibilityJob);
        private readonly IScheduleTaskService _scheduleTaskService;
        #region Fields

        private readonly CategoryType _categoryType = CategoryType.Department;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public ToggleDepartmentVisibilityJob(ISettingService settingService,
            ICategoryService categoryService,
            IStoreContext storeContext,
            IStoreService storeService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
            IScheduleTaskService scheduleTaskService,
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
            _productService = productService;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        protected override void Produce()
        {
            if (_archSettings.DisableCategoryImport)
            {
                var thisTask = _scheduleTaskService.GetTaskByType("Nop.Services.CustomScheduledTasks.ToggleDepartmentVisibilityTask");
                if (thisTask != null)
                {
                    thisTask.Enabled = false;
                    _scheduleTaskService.UpdateTask(thisTask);
                }
                return;
            }

            var departments = _categoryService.GetAllCategories(string.Empty, showHidden: true, categoryType: _categoryType, storeId: RunningOnStoreId);

            Debug($"Producing {departments.Count} departments");
            foreach (var department in departments)
            {
                EnqueueItem(department);
            }
            Debug($"Completed producing");
        }

        protected override void Consume(Category item)
        {
            Debug($"Checking {item.Name} usage");

            var productCategories = _categoryService.GetProductCategoriesByCategoryId(item.Id);
            int productCount = _productService.GetNumberOfProductsInCategory(new List<int> { item.Id }, RunningOnStoreId);

            var childCategoryIds = _categoryService.GetChildCategoryIds(item.Id);
            var productsInChildren = false;
            if (childCategoryIds.Any())
                productsInChildren = ChildCategoriesHaveProducts(childCategoryIds.ToList());
            //Check if child categories have products. In the event of a lower level category, this will return zero and will cover the 
            //use case where products in holding. GetNumberOfProductsInCategory() Only returns products published and not in holding.
            var isCategoryUsed = productCount > 0 || productsInChildren;

            var isPublishedOld = item.Published;
            var includeInTopMenuOld = item.IncludeInTopMenu;

            item.Published = isCategoryUsed;
            item.IncludeInTopMenu = isCategoryUsed;

            var updateRequired = isPublishedOld != item.Published || includeInTopMenuOld != item.IncludeInTopMenu;
            if (updateRequired)
            {
                _categoryService.UpdateCategory(item);
            }

            Debug($"Completed processing {item.Name}");
        }

        private bool ChildCategoriesHaveProducts(List<int> categoryIDs)
        {
            return _productService.GetNumberOfProductsInCategory(categoryIDs, RunningOnStoreId) > 0;
        }

        protected override void CollectData() { }
        #endregion
    }
}
