using System;
using Microsoft.Extensions.Logging;
using Nop.Arch.Services.Helpers;
using Nop.Arch.Services.Payments;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Stores;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    public partial class ExpireCashbackJob : ScheduledJobBase<CustomerWalletTransaction>
    {
        protected override Type TaskType => typeof(ExpireCashbackJob);

        #region Fields

        private readonly ICustomerWalletService _customerWalletService;
        public override bool RunOnlyOnGlobalStore => true;
        #endregion

        #region Ctor

        public ExpireCashbackJob(ISettingService settingService,
            ICategoryService categoryService,
            IStoreContext storeContext,
            IStoreService storeService,
            ICustomerWalletService customerWalletService,
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
            _customerWalletService = customerWalletService;
        }

        #endregion

        #region Methods

        protected override void Produce()
        {
            var cashbackTransactions = _customerWalletService.GetAllCustomerWalletTransactions(
                transactionType: WalletTransactionType.Credit,
                verifiedToArch: true,
                fromDateUtc: DateTime.UtcNow.AddDays(-_archSettings.FetchCashbackFromDaysAgo),
                storeId: 0,
                cashbackStatus: CashbackStatus.Pending);

            Debug($"Producing {cashbackTransactions.Count} cashback transactions");
            foreach (var transaction in cashbackTransactions)
            {
                var isCashback = DateTime.UtcNow.Subtract(transaction.CreatedDateUtc).TotalDays <= _archSettings.ExpireCashbackAfterDays;
                if (!isCashback)
                {
                    EnqueueItem(transaction);
                }
            }
            Debug($"Completed producing");
        }

        protected override void Consume(CustomerWalletTransaction item)
        {
            Debug($"Expiring cashback transaction {item.Id}");

            item.CashbackStatusId = (int)CashbackStatus.Expired;
            _customerWalletService.UpdateCustomerWalletTransaction(item);

            Debug($"Completed processing {item.Id}");
        }

        protected override void CollectData() { }
        #endregion
    }
}
