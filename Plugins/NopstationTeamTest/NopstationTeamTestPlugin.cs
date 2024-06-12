using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.NopstationTeamTest
{
    public class NopstationTeamTestPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public NopstationTeamTestPlugin(IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }


        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Admin.Misc.Employees"] = "Employees",
                ["Admin.Misc.Employees.AddNew"] = "Add new employee",
                ["Admin.Misc.Employees.EditDetails"] = "Edit employee details",
                ["Admin.Misc.Employees.BackToList"] = "back to employee list",
                ["Admin.Misc.Employees"] = "Employees",
                ["Admin.Misc.Employees"] = "Employees",
                ["Admin.Misc.Employee.Fields.Name"] = "Name",
                ["Admin.Misc.Employee.Fields.Designation"] = "Designation",
                ["Admin.Misc.Employee.Fields.IsMVP"] = "Is MVP",
                ["Admin.Misc.Employee.Fields.IsNopCommerceCertified"] = "Is certified",
                ["Admin.Misc.Employee.Fields.EmployeeStatus"] = "Status",
                ["Admin.Misc.Employee.Fields.Name.Hint"] = "Enter employee name.",
                ["Admin.Misc.Employee.Fields.Designation.Hint"] = "Enter employee designation.",
                ["Admin.Misc.Employee.Fields.IsMVP.Hint"] = "Check if employee is MVP.",
                ["Admin.Misc.Employee.Fields.IsNopCommerceCertified.Hint"] = "Check if employee is certified.",
                ["Admin.Misc.Employee.Fields.EmployeeStatus.Hint"] = "Select employee status.",
                ["Admin.Misc.Employee.List.Name"] = "Name",
                ["Admin.Misc.Employee.List.EmployeeStatus"] = "Status",
                ["Admin.Misc.Employee.List.Name.Hint"] = "Search by employee name.",
                ["Admin.Misc.Employee.List.EmployeeStatus.Hint"] = "Search by employee status.",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
           
            await base.UpdateAsync(currentVersion, targetVersion);
        }
      

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Employee/List";
        }
    }
}
