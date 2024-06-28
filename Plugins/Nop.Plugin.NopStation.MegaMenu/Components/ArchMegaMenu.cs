using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;
using Nop.Plugin.NopStation.MegaMenu.Factories;

namespace Nop.Plugin.NopStation.MegaMenu.Components
{
    public class ArchMegaMenuViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly MegaMenuSettings _megaMenuSettings;
        private readonly IMegaMenuModelFactory _megaMenuModelFactory;

        public ArchMegaMenuViewComponent(MegaMenuSettings megaMenuSettings,
            INopStationLicenseService licenseService,
            IMegaMenuModelFactory megaMenuModelFactory)
        {
            _megaMenuSettings = megaMenuSettings;
            _megaMenuModelFactory = megaMenuModelFactory;
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke()
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var model = _megaMenuModelFactory.PrepareMegaMenuModelAsync();
            return View(model);
        }
    }
}
