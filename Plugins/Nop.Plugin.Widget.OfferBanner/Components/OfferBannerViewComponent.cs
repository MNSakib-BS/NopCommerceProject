using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widget.OfferBanner.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widget.OfferBanner.Components
{
    public class OfferBannerViewComponent : NopViewComponent
    {
        private ISettingService _settingService;
        public OfferBannerViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var offserSettings = _settingService.LoadSetting<OfferBannerSettings>();
            var model = new ConfigurationModel()
            {
                Message = offserSettings.Message
            };

            return View(model);
        }
    }
}
