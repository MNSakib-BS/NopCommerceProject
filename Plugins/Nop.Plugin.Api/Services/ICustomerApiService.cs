using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Arch.Core.Domains.Customers;

namespace Nop.Plugin.Api.Services
{
    public interface ICustomerApiService
    {
        Task<int> GetCustomersCountAsync();

        Task<CustomerDto> GetCustomerByIdAsync(int id, bool showDeleted = false);

        Task<Customer> GetCustomerEntityByIdAsync(int id);

        Task<IList<CustomerDto>> GetCustomersDtosAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            int sinceId = Constants.Configurations.DefaultSinceId);

        Task<IList<CustomerDto>> SearchAsync(
            string query = "", string order = Constants.Configurations.DefaultOrder,
            int page = Constants.Configurations.DefaultPageValue, int limit = Constants.Configurations.DefaultLimit);

        Task<Dictionary<string, string>> GetFirstAndLastNameByCustomerIdAsync(int customerId);

        Task<Customer> GetCustomerByEmailAsync(string email);

        Task<Customer> GetCustomerByUsernameAsync(string username);

        Task<CustomerRole> GetCustomerRoleBySystemNameAsync(string systemName);

        Task AddCustomerRoleMappingAsync(CustomerCustomerRoleMapping roleMapping);

        Task<CustomerPicture> GetCustomerPictureMappingAsync(int customerId);
    }
}
