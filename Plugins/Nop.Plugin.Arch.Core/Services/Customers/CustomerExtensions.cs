using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Services.Common;
using NopCustomerDefaults = Nop.Plugin.Arch.Core.Domains.Customers.NopCustomerDefaults;

namespace Nop.Plugin.Arch.Core.Services.Customers;
/// <summary>
/// Customer extensions
/// </summary>
public static class CustomerExtensions
{
    public static async Task<bool> IsDebtor(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.IsDebtorAttribute, storeId);
    }

    public static async Task<string> DebtorNumber(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.DebtorNumberAttribute, storeId);
    }

    public static async Task<int> DebtorErrorCode(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.DebtorErrorCodeAttribute, storeId);
    }

    public static async Task<string> DebtorAccountName(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.DebtorAccountNameAttribute, storeId);
    }

    public static async Task<DebtorAccountStatus> DebtorAccountStatus(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        var statusId = await genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.DebtorAccountStatusAttribute, storeId);
        return (DebtorAccountStatus)statusId;
    }

    public static async Task<DebtorType> DebtorType(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        var typeId = await genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.DebtorTypeAttribute, storeId);
        return (DebtorType)typeId;
    }

    public static async Task<int> PriceTier(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.PriceTierAttribute, storeId);
    }

    public static async Task<int> PriceNumber(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.PriceNumberAttribute, storeId);
    }

    public static async Task<byte> DebtorDeliveryMethod(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<byte>(customer, NopCustomerDefaults.DebtorDeliveryMethod, storeId);
    }

    public static async Task<bool> DebtorDeliveryChargeValueType(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.DebtorDeliveryChargeValueType, storeId);
    }

    public static async Task<decimal> DebtorDeliveryChargeValue(this Customer customer, int storeId)
    {
        var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        return await genericAttributeService.GetAttributeAsync<decimal>(customer, NopCustomerDefaults.DebtorDeliveryChargeValue, storeId);
    }
}
