using System;
using Microsoft.Extensions.Logging;
using Arch.Core.Services.Helpers;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using ILogger = Nop.Services.Logging.ILogger;

namespace Arch.Core.Infrastructure.JobSchedules.Jobs
{
    public partial class DepartmentHierarchyJob : ScheduledJobBase<Category>
    {
        protected override Type TaskType => typeof(DepartmentHierarchyJob);

        #region Fields

        private readonly CategoryType _categoryType = CategoryType.Department;
        private readonly ICategoryService _categoryService;
        private readonly IScheduleTaskService _scheduleTaskService;
        #endregion

        #region Ctor

        public DepartmentHierarchyJob(ISettingService settingService,
            ICategoryService categoryService,
            IStoreContext storeContext,
            IStoreService storeService,
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
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        protected override void Produce()
        {
            if (_archSettings.DisableCategoryImport)
            {
                return;
            }

            var departments = _categoryService.GetAllCategoriesByType(_categoryType, RunningOnStoreId);

            Debug($"Producing {departments.Count} departments");
            foreach (var department in departments)
            {
                if (department.ExternalId != department.ExternalParentId)
                {
                    EnqueueItem(department);
                }
            }
            Debug($"Completed producing");
        }

        protected override void Consume(Category item)
        {
            Debug($"Updating {item.Name} ParentCategoryId");

            var foundParent = _categoryService.GetCategoryByExternalId(item.ExternalParentId, _categoryType, storeId: RunningOnStoreId, storeTypeId: item.StoreTypeId);
            item.ParentCategoryId = foundParent?.Id ?? 0;

            _categoryService.UpdateCategory(item);

            Debug($"Completed processing {item.Name} ParentCategoryId");
        }

        protected override void CollectData() { }
        #endregion
    }
}
