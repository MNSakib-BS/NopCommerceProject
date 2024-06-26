using Nop.Arch.Domain.AbandonedCartReminder;
using Nop.Arch.Domain.StoreType;
using Nop.Arch.Models.AbandonedCartReminder;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.AbandonedCartReminder.Services
{
    public class AbandonedCartReminderService : IAbandonedCartReminderService
    {
        private readonly IRepository<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder> _abandonedCartReminderRepository;
        private readonly IRepository<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue> _abandonedCartReminderQueueRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly CachingSettings _cachingSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        public AbandonedCartReminderService(IRepository<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder> abandonedCartReminderRepository,
                                            IRepository<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue> abandonedCartReminderQueueRepository,
                                            IEventPublisher eventPublisher,
                                            IShoppingCartService shoppingCartService,
                                            IStoreMappingService storeMappingService,
                                            IBaseAdminModelFactory baseAdminModelFactory,
                                            IWorkContext workContext,
                                            IStoreContext storeContext,
                                            ICustomerService customerService,
                                            IRepository<StoreMapping> storeMappingRepository,
                                            CachingSettings cachingSettings)
        {
            _abandonedCartReminderRepository = abandonedCartReminderRepository;
            _abandonedCartReminderQueueRepository = abandonedCartReminderQueueRepository;
            _shoppingCartService = shoppingCartService;
            _storeMappingService = storeMappingService;
            _cachingSettings = cachingSettings;
            _eventPublisher = eventPublisher;
            _baseAdminModelFactory = baseAdminModelFactory;
            _workContext = workContext;
            _storeContext = storeContext;
            _customerService = customerService;
            _storeMappingRepository = storeMappingRepository;
        }

        public async Task<AbandonedCartReminderModel> CreateAsync(AbandonedCartReminderModel model)
        {
            var entity = model.ToEntity<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>();
            await _abandonedCartReminderRepository.InsertAsync(entity);
            return model;
        }

        public async Task<AbandonedCartReminderModel> AddFormAsync(AbandonedCartReminderModel model)
        {
            var form = model.ToEntity<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>();
            await _abandonedCartReminderRepository.InsertAsync(form);
            return form.ToModel<AbandonedCartReminderModel>();
        }

        public async Task<AbandonedCartReminderModel> EditFormAsync(AbandonedCartReminderModel model)
        {
            var form = model.ToEntity<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>();
            await _abandonedCartReminderRepository.UpdateAsync(form);

            model = form.ToModel<AbandonedCartReminderModel>();
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

            return model;
        }

        public async Task<AbandonedCartReminderModel> SubmitFormAsync(AbandonedCartReminderModel model)
        {
            var item = model.ToEntity<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>();
            await _abandonedCartReminderRepository.InsertAsync(item);
            return item.ToModel<AbandonedCartReminderModel>();
        }

        public async Task DeleteAbandonedCartReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder abandonedCartReminder)
        {
            if (abandonedCartReminder == null)
                throw new ArgumentNullException(nameof(abandonedCartReminder));

            await _abandonedCartReminderRepository.DeleteAsync(abandonedCartReminder);
        }

        public async Task<AbandonedCartReminderModel> GetAbandonedCartReminderFormAsync(int id)
        {
            var entity = await _abandonedCartReminderRepository.GetByIdAsync(id);
            var entityModel = entity?.ToModel<AbandonedCartReminderModel>();
            await _baseAdminModelFactory.PrepareStoresAsync(entityModel.AvailableStores);
            return entityModel;
        }

        public Task<AbandonedCartReminderSearchModel> PrepareSearchModelAsync(AbandonedCartReminderSearchModel searchModel)
        {
            searchModel.SetGridPageSize();
            return Task.FromResult(searchModel);
        }

        public async Task<AbandonedCartReminderListModel> PrepareFormListModelAsync(AbandonedCartReminderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var forms = await SearchAsync(searchModel);

            var model = new AbandonedCartReminderListModel().PrepareToGrid(searchModel, forms, () =>
            {
                return forms.Select(item =>
                {
                    var formModel = new AbandonedCartReminderGridModel
                    {
                        Id = item.Id,
                        Description = item.Description,
                        DelayBeforeSend = item.DelayBeforeSend,
                        Active = item.Active,
                        DelayPeriod = item.DelayPeriod,
                        DelayPeriodId = item.DelayPeriodId
                    };

                    return formModel;
                });
            });

            return model;
        }

        public async Task<IPagedList<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>> SearchAsync(AbandonedCartReminderSearchModel model, int pageIndex = 0, int pageSize = 15)
        {
            var query = _abandonedCartReminderRepository.Table;
            return await Task.FromResult(new PagedList<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>(query, pageIndex, pageSize, false));
        }

        public async Task<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder> GetAbandonedCartReminderByIdAsync(int abandonedCartReminderId)
        {
            if (abandonedCartReminderId == 0)
                return null;

            return await _abandonedCartReminderRepository.ToCachedGetByIdAsync(abandonedCartReminderId, _cachingSettings.ShortTermCacheTime);
        }

        /// <summary>
        /// Updates the reminder
        /// </summary>
        /// <param name="reminder">Reminder</param>
        public async Task UpdateReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder reminder)
        {
            if (reminder == null)
                throw new ArgumentNullException(nameof(reminder));

            await _abandonedCartReminderRepository.UpdateAsync(reminder);
            await _eventPublisher.EntityUpdatedAsync(reminder);
        }

        public async Task<AbandonedCartReminderModel> PrepareReminderModelAsync(AbandonedCartReminderModel model)
        {
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
            return model;
        }

        #region "Scheduled reminder"

        public async Task<List<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>> GetActiveRemindersAsync()
        {
            var reminders = await (from item in _abandonedCartReminderRepository.Table
                                   where item.Active && item.DelayBeforeSend > 0
                                   select item).ToListAsync();
            return reminders;
        }

        public async Task<List<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue>> GetActiveReminderQueueAsync()
        {
            var queue = await (from item in _abandonedCartReminderQueueRepository.Table
                               where item.IsActive
                               select item).ToListAsync();
            return queue;
        }

        public async Task SetReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue reminderQueue)
        {
            var currentReminder = await (from item in _abandonedCartReminderRepository.Table
                                         where item.Id == reminderQueue.ReminderId
                                         select item).FirstOrDefaultAsync();

            var reminder = await (from item in _abandonedCartReminderRepository.Table.Where(x => x.Id != currentReminder.Id && x.DelayPeriodId >= currentReminder.DelayPeriodId && x.DelayBeforeSend > currentReminder.DelayBeforeSend && x.Active)
                                  join mapping in _storeMappingRepository.Table.Where(x => x.EntityName == "AbandonedCartReminder" && x.StoreId == reminderQueue.StoreId) on item.Id equals mapping.EntityId into records
                                  from mapping in records.DefaultIfEmpty()
                                  select item).OrderBy(x => x.DelayPeriodId).ThenBy(x => x.DelayBeforeSend).FirstOrDefaultAsync();

            if (reminder != null)
            {
                reminderQueue.ReminderId = reminder.Id;
            }
            else
            {
                reminderQueue.IsActive = false;
            }

            await _abandonedCartReminderQueueRepository.UpdateAsync(reminderQueue);
        }

        #endregion
    }
}
