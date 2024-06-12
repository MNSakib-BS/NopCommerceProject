using FluentValidation;
using Nop.Plugin.Widgets.OurTeam.Domain;
using Nop.Plugin.Widgets.OurTeam.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.OurTeam.Validators
{
    public partial class EmployeesValidator : BaseNopValidator<EmployeeModel>
    {
        public EmployeesValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Misc.Employee.Fields.Name.Required"));
            RuleFor(x => x.Designation).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Misc.Employee.Fields.Designation.Required"));

            SetDatabaseValidationRules<Employees>();
        }
    }

}
