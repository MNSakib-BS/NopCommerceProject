using FluentValidation;
using Nop.Plugin.NopStation.Core.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.Core.Validators
{
    public class LocaleResourceValidator : BaseNopValidator<CoreLocaleResourceModel>
    {
        public LocaleResourceValidator()
        {
            RuleFor(x => x.ResourceName).NotEmpty().WithMessage("Admin.Configuration.Languages.Resources.Fields.Name.Required");
        }
    }
}
