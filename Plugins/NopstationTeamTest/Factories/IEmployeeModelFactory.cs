using Nop.Plugin.Misc.NopstationTeamTest.Domain;
using Nop.Plugin.Misc.NopstationTeamTest.Models;

namespace Nop.Plugin.Misc.NopstationTeamTest.Factories
{
    public interface IEmployeeModelFactory
    {
        Task<EmployeeListModel> PrepareEmployeeListModelAsyc(EmployeeSearchModel searchModel);
        Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsyc(EmployeeSearchModel searchModel);
        Task<EmployeeModel> PrepareEmployeeModelAsyc(EmployeeModel model, Employee employee, bool excludeProperties = false);

    }
}