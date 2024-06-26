using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Theme.Arch.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.Arch.Components
{
    public class ArchViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly ArchThemeSettings _ArchSettings;

        public ArchViewComponent(INopStationLicenseService licenseService,
            ArchThemeSettings ArchSettings)
        {
            _licenseService = licenseService;
            _ArchSettings = ArchSettings;
        }

        public IViewComponentResult Invoke(string widgetZone)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var model = new PublicModel()
            {
                CustomCss = _ArchSettings.CustomCss,
                EnableImageLazyLoad = _ArchSettings.EnableImageLazyLoad
            };

            return View(model);
        }
    }
}
