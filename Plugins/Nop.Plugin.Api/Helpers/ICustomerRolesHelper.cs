using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Helpers
{
    public interface ICustomerRolesHelper
    {
        Task<IList<CustomerRole>> GetValidCustomerRolesAsync(List<int> roleIds);
        Task<bool> IsInGuestsRoleAsync(IList<CustomerRole> customerRoles);
        Task<bool> IsInRegisteredRoleAsync(IList<CustomerRole> customerRoles);
    }
}
