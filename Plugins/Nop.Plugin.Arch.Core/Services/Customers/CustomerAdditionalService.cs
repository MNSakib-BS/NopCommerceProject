using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Api;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Services.Stores;
using Nop.Services.Common;
using Nop.Services.Stores;
using NopCustomerDefaults = Nop.Plugin.Arch.Core.Domains.Customers.NopCustomerDefaults;

namespace Nop.Plugin.Arch.Core.Services.Customers;
public class CustomerAdditionalService : ICustomerAdditionalService
{
    #region Fields

    private readonly IRepository<CustomerAdditional> _customerAdditionalRepository;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IStoreService _storeService;
    private readonly IStoreAdditionalService _storeAdditionalService;
    private readonly ArchApiSettings _archApiSettings;


    #endregion

    #region Ctor
    public CustomerAdditionalService(IRepository<CustomerAdditional> customerAdditionalRepository,
        IGenericAttributeService genericAttributeService,
        IStoreService storeService,
        IStoreAdditionalService storeAdditionalService,
        ArchApiSettings archApiSettings
        )
    {
        _customerAdditionalRepository = customerAdditionalRepository;
        _genericAttributeService = genericAttributeService;
        _storeService = storeService;
        _storeAdditionalService = storeAdditionalService;
        _archApiSettings = archApiSettings;
    }

    #endregion


    #region Utilities

    private async Task DeleteDebtorGenericAttributeAsync(Customer customer, string key, int storeId)
    {
        var attributes =await _genericAttributeService
            .GetAttributesForEntityAsync(customer.Id, nameof(Customer)).Result
            .Where(i => i.Key == key && i.StoreId == storeId)
            .ToListAsync();

       await _genericAttributeService.DeleteAttributesAsync(attributes);
    }

    private async Task UpdatePriceNumberAsync(Customer customer, string priceNumber, int storeId, bool? archApiDebtorCallSuccess = null)
    {
        var storeAdditional = await _storeAdditionalService.GetStoreAddiitonalByStoreIdAsync(storeId);

        var defaultPriceTier = storeAdditional?.DefaultPriceTierId ??
                               NopCustomerDefaults.PriceTierDefault;   

        if (await customer.IsDebtor(storeId))
        {
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PriceNumberAttribute, Convert.ToInt32(priceNumber), storeId);
        }

        if (string.IsNullOrWhiteSpace(priceNumber))
        {
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PriceTierAttribute, defaultPriceTier, storeId);
            return;
        }

        var updatedPriceTier = !archApiDebtorCallSuccess.HasValue || !archApiDebtorCallSuccess.Value
            ? defaultPriceTier
            : Convert.ToInt32(priceNumber);

       await  _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PriceTierAttribute, updatedPriceTier, storeId);
    }


    #endregion

    #region Methods

    public virtual async Task<CustomerAdditional> GetCustomerAddiitonalByCustomerIdAsync(int customerId)
    {
        return await _customerAdditionalRepository.Table.Where(e => e.CustomerId == customerId).FirstOrDefaultAsync();
    }

    public virtual async Task UpdateDebtorInformationAsync(Customer customer,
            string debtorNumber,
            int priceNumber,
            string accountName,
            int debtorType,
            string status,
            int errorCode,
            int deliveryMethod,
            bool deliveryChargeValueType,
            decimal deliveryChargeValue,
            int storeId,
            bool archApiDebtorCallSuccess)
    {
        if (archApiDebtorCallSuccess)
        {
            await UpdatePriceNumberAsync(customer, priceNumber.ToString(), storeId, archApiDebtorCallSuccess);

            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.IsDebtorAttribute, true, storeId);

            if (!string.IsNullOrEmpty(debtorNumber) && debtorNumber != _archApiSettings.DefaultDebtorCode)
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorNumberAttribute, debtorNumber, storeId);
            else
                await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorNumberAttribute, storeId);

           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorAccountNameAttribute, accountName, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorTypeAttribute, debtorType, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorAccountStatusAttribute, status, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorErrorCodeAttribute, errorCode, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryMethod, deliveryMethod, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryChargeValueType, deliveryChargeValueType, storeId);
           await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryChargeValue, deliveryChargeValue, storeId);
        }
        else
        {
            await ClearDebtorInformationAsync(customer, storeId, archApiDebtorCallSuccess);
        }
    }


    public virtual async Task ClearDebtorInformationAsync(Customer customer, int storeId, bool? archApiDebtorCallSuccess = null)
    {
        var attributes = await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, nameof(Customer));
        var priceTierAttribute = attributes.FirstOrDefault(i => i.Key == NopCustomerDefaults.PriceTierAttribute && i.StoreId == storeId);

        await UpdatePriceNumberAsync(customer, priceTierAttribute?.Value, storeId, archApiDebtorCallSuccess);

        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorNumberAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.IsDebtorAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorAccountNameAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorTypeAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorAccountStatusAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorErrorCodeAttribute, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryMethod, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryChargeValueType, storeId);
        await DeleteDebtorGenericAttributeAsync(customer, NopCustomerDefaults.DebtorDeliveryChargeValue, storeId);
    }


    #endregion


}
