using Nop.Plugin.Widgets.OurTeam.Domain;
using Nop.Plugin.Widgets.OurTeam.Models;

namespace Nop.Plugin.Widgets.OurTeam.Factories
{
    public interface IEmployeesModelFactory
    {
        Task<EmployeeListModel> PrepareEmployeeListModelAsyc(EmployeeSearchModel searchModel);
        Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsyc(EmployeeSearchModel searchModel);
        Task<EmployeeModel> PrepareEmployeeModelAsyc(EmployeeModel model, Employees employee, bool excludeProperties = false);
        Task<List<EmployeeModel>> PrepareAllEmployeesAsync();
    }
}