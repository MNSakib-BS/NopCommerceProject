using Nop.Core;
using Nop.Plugin.Misc.NopstationTeamTest.Domain;

namespace Nop.Plugin.Misc.NopstationTeamTest.Services;
public interface IEmployeeService
{

    Task InsertEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(Employee employee);
    Task<Employee> GetEmployeeByIdAsync(int employeeId);
    Task UpdateEmployeeAsync(Employee employee);
    Task<IPagedList<Employee>> SearchEmployeesAsync(string Name, int statusId, int pageIndex = 0,
            int pageSize = int.MaxValue);


}
