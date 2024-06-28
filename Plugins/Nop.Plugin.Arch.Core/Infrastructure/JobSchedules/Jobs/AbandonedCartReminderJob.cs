using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders;
using Nop.Plugin.Arch.Core.Services.Helpers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules.Jobs
{
    public class AbandonedCartReminderJob : ScheduledJobBase<AbandonedCartReminderQueue>
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;

        public AbandonedCartReminderJob(ISettingService settingService,
            IStoreService storeService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            IObjectConverter objectConverter,
            ILogger<ScheduledJobBase<object>> jobLogger,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext) : base(settingService, storeService, storeContext, storeMappingService, urlRecordService, logger, objectConverter, jobLogger)
        {
            _customerService = customerService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
        }

        protected override Type TaskType => typeof(AbandonedCartReminderJob);

       

        public override bool RunOnlyOnGlobalStore => true;

       

        protected override void Produce()
        {
            var reminders = GetActiveReminders();
            if (reminders != null && reminders.Any())
            {
                var reminderQueue = GetActiveReminderQueue();
                if (reminderQueue != null && reminderQueue.Any())
                {
                    foreach (var queue in reminderQueue)
                    {
                        var currentReminder = (from item in reminders where item.Id == queue.ReminderId && item.Active select item).FirstOrDefault();
                        if (currentReminder != null)
                        {
                            DateTime checkTime = DateTime.Now;
                            if (currentReminder.DelayPeriod == Nop.Core.Domain.Messages.MessageDelayPeriod.Hours)
                            {
                                checkTime = queue.CartLastUpdated.AddHours((double)currentReminder.DelayBeforeSend);
                            }
                            else
                            {
                                checkTime = queue.CartLastUpdated.AddDays((double)currentReminder.DelayBeforeSend);
                            }

                            if (DateTime.Now >= checkTime)
                            {
                                EnqueueItem(queue);
                            }
                        }
                    }
                }
            }

            Debug($"Completed Producing");
        }

        protected override async void Consume(AbandonedCartReminderQueue item)
        {
            var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
            if (customer != null)
            {
                if (!string.IsNullOrEmpty(customer.Email))
                {
                   await _workflowMessageService.SendCustomerAbandonedCartNotificationMessage(customer, item.StoreId, await _workContext.GetWorkingLanguageAsync());
                    SetReminder(item);
                }
            }
        }

        protected override void CollectData() { }

        #region "Methods"
        public List<AbandonedCartReminder> GetActiveReminders()
        {
            var reminders = (from item in AbandonedCartReminderRepository.Table where item.Active && item.DelayBeforeSend > 0 select item).ToList();
            return reminders;
        }

        public List<Arch.Core.Domain.AbandonedCartReminder.AbandonedCartReminderQueue> GetActiveReminderQueue()
        {
            var queue = (from item in AbandonedCartReminderQueueRepository.Table where item.IsActive select item).ToList();
            return queue;
        }

        public void SetReminder(Arch.Core.Domain.AbandonedCartReminder.AbandonedCartReminderQueue reminderQueue)
        {
            var currentReminder = (from item in AbandonedCartReminderRepository.Table where item.Id == reminderQueue.ReminderId select item).FirstOrDefault();
            AbandonedCartReminder reminder = null;

            reminder = (from item in AbandonedCartReminderRepository.Table.Where(x => x.Id != currentReminder.Id && x.DelayPeriodId == currentReminder.DelayPeriodId && x.DelayBeforeSend > currentReminder.DelayBeforeSend && x.Active)
                        join mapping in StoreMappingRepository.Table.Where(x => x.EntityName == "AbandonedCartReminder" && x.StoreId == reminderQueue.StoreId) on item.Id equals mapping.EntityId into records
                        from mapping in records.DefaultIfEmpty()
                        select item).OrderBy(x => x.DelayPeriodId).ThenBy(x => x.DelayBeforeSend).FirstOrDefault();

            if (currentReminder.DelayPeriod == Core.Domain.Messages.MessageDelayPeriod.Hours)
            {
                if (reminder == null)
                {
                    reminder = (from item in AbandonedCartReminderRepository.Table.Where(x => x.Id != currentReminder.Id && x.DelayPeriodId > currentReminder.DelayPeriodId && x.Active)
                                join mapping in StoreMappingRepository.Table.Where(x => x.EntityName == "AbandonedCartReminder" && x.StoreId == reminderQueue.StoreId) on item.Id equals mapping.EntityId into records
                                from mapping in records.DefaultIfEmpty()
                                select item).OrderBy(x => x.DelayPeriodId).ThenBy(x => x.DelayBeforeSend).FirstOrDefault();
                }
            }

            if (reminder != null)
            {
                reminderQueue.ReminderId = reminder.Id;
            }
            else
            { reminderQueue.IsActive = false; }
            AbandonedCartReminderQueueRepository.Update(reminderQueue);
        }
        #endregion
    }
}
