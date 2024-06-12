using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Widgets.OurTeam.Domain;

namespace Nop.Plugin.Widgets.OurTeam.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly IRepository<Employees> _employeeRepository;

        public EmployeesService(IRepository<Employees> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

      
        public virtual async Task InsertEmployeeAsync(Employees employee)
        {
            await _employeeRepository.InsertAsync(employee);
        }
        public virtual async Task UpdateEmployeeAsync(Employees employee)
        {
            await _employeeRepository.UpdateAsync(employee);
        }
        public virtual async Task DeleteEmployeeAsync(Employees employee)
        {
            await _employeeRepository.DeleteAsync(employee);
        }
        public async Task<IList<Employees>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAll().ToListAsync();
        }
        public virtual async Task<Employees> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetByIdAsync(employeeId);
        }
        public virtual async Task<IPagedList<Employees>> SearchEmployeesAsync(string Name , int statusId, int pageIndex= 0,
            int pageSize= int.MaxValue)
        {
            //return await _employeeRepository.GetAllPagedAsync();
            var query = from e in _employeeRepository.Table
                        select e;
            if(!string.IsNullOrEmpty(Name) )
                query= query.Where(e => e.Name.Contains(Name));
            if(statusId > 0)
                query = query.Where(e => e.EmployeeStatusId == statusId);   
            query = query.OrderBy(e => e.Name);
            return await query.ToPagedListAsync(pageIndex, pageSize);
             

        }
    }
}
