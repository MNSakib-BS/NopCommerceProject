using FluentValidation;
using Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Validators
{
    public class CategoryIconValidator : BaseNopValidator<CategoryIconModel>
    {
       
        public CategoryIconValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PictureId)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.NopStation.MegaMenu.CategoryIcons.Fields.Picture.Required"));
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.NopStation.MegaMenu.CategoryIcons.Fields.Category.Required"));
        }
    }
}
