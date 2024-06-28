using Nop.Core.Domain.Stores;
using Nop.Core;
using Nop.Core.Domain.WishlistItemSaleNotifier;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Orders;
using Nop.Arch.Services.Helpers;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    public class WishlistItemSaleNotifierJob : ScheduledJobBase<WishlistItemSaleNotifierQueue>
    {
        protected override Type TaskType => typeof(WishlistItemSaleNotifierJob);

        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IRepository<WishlistItemSaleNotifier> WishlistItemSaleNotifierRepository;
        private readonly IRepository<WishlistItemSaleNotifierQueue> WishlistItemSaleNotifierQueueRepository;
        private readonly IRepository<StoreMapping> StoreMappingRepository;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;

        public override bool RunOnlyOnGlobalStore => true;

        public WishlistItemSaleNotifierJob(ICustomerService customerService,
                                          IGenericAttributeService genericAttributeService,
                                          ISettingService settingService,
                                          IStoreMappingService storeMappingService,
                                          IStoreService storeService,
                                          IStoreContext storeContext,
                                          IProductService productService,
                                          IShoppingCartService shoppingCartService,
                                          IUrlRecordService urlRecordService,
                                          IWorkflowMessageService workflowMessageService,
                                          IWorkContext workContext,
                                          ILogger logger,
                                          IObjectConverter objectConverter,
                                          IRepository<WishlistItemSaleNotifier> wishlistItemSaleNotifierRepository,
                                          IRepository<WishlistItemSaleNotifierQueue> wishlistItemSaleNotifierQueueRepository,
                                          IRepository<StoreMapping> storeMappingRepository,
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
            WishlistItemSaleNotifierRepository = wishlistItemSaleNotifierRepository;
            WishlistItemSaleNotifierQueueRepository = wishlistItemSaleNotifierQueueRepository;
            StoreMappingRepository = storeMappingRepository;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
        }

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

        protected override void Consume(WishlistItemSaleNotifierQueue item)
        {
            var customer = _customerService.GetCustomerById(item.CustomerId);
            var cart = _shoppingCartService.GetShoppingCart(customer, null, false, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);
            var productIds = new List<int>();

            if (customer != null)
            {
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    foreach(var cartItem in cart)
                    {
                        var product = _productService.GetProductById(cartItem.ProductId);
                        if (product.OnPromotionField) 
                        {
                            productIds.Add(product.Id);
                        }
                    }

                    if (productIds.Any())
                    {
                        _workflowMessageService.SendCustomerWishlistItemSaleNotificationMessage(customer, productIds, item.StoreId, _workContext.WorkingLanguage.Id);
                        SetReminder(item);
                    }

                }
            }
        }

        protected override void CollectData() { }

        #region "Methods"
        public List<WishlistItemSaleNotifier> GetActiveReminders()
        {
            var reminders = (from item in WishlistItemSaleNotifierRepository.Table where item.Active && item.DelayBeforeSend > 0 select item).ToList();
            return reminders;
        }

        public List<WishlistItemSaleNotifierQueue> GetActiveReminderQueue()
        {
            var queue = (from item in WishlistItemSaleNotifierQueueRepository.Table where item.IsActive select item).ToList();
            return queue;
        }

        public void SetReminder(WishlistItemSaleNotifierQueue wishlistQueue)
        {
            var currentReminder = (from item in WishlistItemSaleNotifierRepository.Table where item.Id == wishlistQueue.ReminderId select item).FirstOrDefault();
            WishlistItemSaleNotifier reminder = null;

            reminder = (from item in WishlistItemSaleNotifierRepository.Table.Where(x => x.Id != currentReminder.Id && x.DelayPeriodId == currentReminder.DelayPeriodId && x.DelayBeforeSend > currentReminder.DelayBeforeSend && x.Active)
                        join mapping in StoreMappingRepository.Table.Where(x => x.EntityName == "WishlistItemSaleNotifier" && x.StoreId == wishlistQueue.StoreId) on item.Id equals mapping.EntityId into records
                        from mapping in records.DefaultIfEmpty()
                        select item).OrderBy(x => x.DelayPeriodId).ThenBy(x => x.DelayBeforeSend).FirstOrDefault();

            if (currentReminder.DelayPeriod == Core.Domain.Messages.MessageDelayPeriod.Hours)
            {
                if (reminder == null)
                {
                    reminder = (from item in WishlistItemSaleNotifierRepository.Table.Where(x => x.Id != currentReminder.Id && x.DelayPeriodId > currentReminder.DelayPeriodId && x.Active)
                                join mapping in StoreMappingRepository.Table.Where(x => x.EntityName == "WishlistItemSaleNotifier" && x.StoreId == wishlistQueue.StoreId) on item.Id equals mapping.EntityId into records
                                from mapping in records.DefaultIfEmpty()
                                select item).OrderBy(x => x.DelayPeriodId).ThenBy(x => x.DelayBeforeSend).FirstOrDefault();
                }
            }

            if (reminder != null)
            {
                wishlistQueue.ReminderId = reminder.Id;
            }
            else { wishlistQueue.IsActive = false; }
            WishlistItemSaleNotifierQueueRepository.Update(wishlistQueue);
        }
        #endregion
    }
}

