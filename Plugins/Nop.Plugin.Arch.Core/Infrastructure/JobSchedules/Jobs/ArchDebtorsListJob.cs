using System;
using ArchServiceReference;
using Microsoft.Extensions.Logging;
using Arch.Core.Services.Helpers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;

using Nop.Services.Seo;
using Nop.Services.Stores;
using ILogger = Nop.Services.Logging.ILogger;

namespace Arch.Core.Infrastructure.JobSchedules.Jobs
{
    /// <summary>
    /// Represents a task for calling the arch api and resolving the debtors list
    /// </summary>
    public class ArchDebtorListJob : ArchScheduledJobBase<GetDebtorsResponse.DebtorElement>
    {
        protected override Type TaskType => typeof(ArchDebtorListJob);

        private const string LastUpdateSettingParam = "ArchDebtorsSalesOrderListTask_LastUpdate";

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;

        public ArchDebtorListJob(
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
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
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
        }

        protected override void Produce()
        {
            var lastUpdate = GetLastUpdate(LastUpdateSettingParam);

            Debug($"Calling ArchAPI / GetDebtorList");
            var response = ArchClient.GetDebtorListAsync(new GetDebtorsRequest()
            {
                LastUpdate = lastUpdate,
                SystemAuthenticationCode = _archSettings.SystemAuthenticationCode,
            });

            if (response.Result != null)
            {
                if (!response.Result.Success)
                {
                    Debug(response.Result.ResponseMessage);
                    return;
                }

                var data = response.Result.List;

                var count = data.Length;
                Debug($"Producing {count} items");
                for (var i = 0; i < count; i++)
                {
                    var item = data[i];

                    EnqueueItem(item);
                }
            }

            Debug($"Completed Producing");
            SetLastUpdate(LastUpdateSettingParam);
        }

        protected override void Consume(GetDebtorsResponse.DebtorElement item)
        {
            Debug($"Consuming {item.AccountName}");

            var debtorNumber = _genericAttributeService.GetAttributesByKeyValue(NopCustomerDefaults.DebtorNumberAttribute,
                item.DebtorNumber,
                nameof(Customer),
                RunningOnStoreId);

            if (debtorNumber.Count == 0)
                return;

            foreach (GenericAttribute genericAttribute in debtorNumber)
            {
                Customer customer = _customerService.GetCustomerById(genericAttribute.EntityId);
                if (customer == null)
                    return;

                _customerService.UpdateDebtorInformation(customer,
                    item.DebtorNumber,
                    Convert.ToInt32(item.PriceNumber),
                    item.AccountName,
                    item.DebtorType,
                    item.Status,
                    item.ErrorCode,
                    item.DeliveryMethod,
                    item.DeliveryChargeValueType,
                    item.DeliveryChargeValue,
                    RunningOnStoreId,
                    archApiDebtorCallSuccess: true);

                _customerService.UpsertCustomerShippingAddress(customer, item.DeliveryAddress1, item.DeliveryAddress2, item.DeliveryPostalCode);
            }

            Debug($"Completed processing {item.AccountName}");
        }

        protected override void CollectData() { }
    }
}
