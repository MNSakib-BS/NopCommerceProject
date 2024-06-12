using Nop.Core;
using Nop.Plugin.Widgets.OurTeam.Domain;

namespace Nop.Plugin.Widgets.OurTeam.Services;
public interface IEmployeesService
{

    Task InsertEmployeeAsync(Employees employee);
    Task DeleteEmployeeAsync(Employees employee);
    Task<Employees> GetEmployeeByIdAsync(int employeeId);
    Task UpdateEmployeeAsync(Employees employee);
    Task<IPagedList<Employees>> SearchEmployeesAsync(string Name, int statusId, int pageIndex = 0,
            int pageSize = int.MaxValue);
    Task<IList<Employees>> GetAllEmployeesAsync();


}
