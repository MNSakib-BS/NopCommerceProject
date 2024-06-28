using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Arch.Core.Domains.Affiliates;
using Nop.Plugin.Arch.Core.Domains.Customers;

namespace Nop.Plugin.Arch.Core.Services.Customers;
public interface ICustomerAdditionalService
{
    Task<CustomerAdditional> GetCustomerAddiitonalByCustomerIdAsync(int customerId);
    Task UpdateDebtorInformationAsync(Customer customer,
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
            bool archApiDebtorCallSuccess);
    Task ClearDebtorInformationAsync(Customer customer, int storeId, bool? archApiDebtorCallSuccess = null);
}
