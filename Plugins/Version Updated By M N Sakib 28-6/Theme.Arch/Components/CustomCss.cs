using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Theme.Arch.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.NopStation.Theme.Arch.Components
{
    public class CustomCssViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IThemeContext _themeContext;
        private readonly CommonSettings _commonSettings;
        private readonly IStoreContext _storeContext;
        private readonly INopFileProvider _nopFileProvider;

        #endregion

        #region Ctor

        public CustomCssViewComponent(
            ILogger logger,
            ISettingService settingService,
            IThemeContext themeContext, 
            CommonSettings commonSettings,
            IStoreContext storeContext,
            INopFileProvider nopFileProvider)
        {
            _logger = logger;
            _settingService = settingService;
            _themeContext = themeContext;
            _commonSettings = commonSettings;
            _storeContext = storeContext;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Methods
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (string.Equals(widgetZone, ArchThemeDefaults.CustomCssWidgetZone, StringComparison.InvariantCultureIgnoreCase))
            {
                var workingThemeName = await _themeContext.GetWorkingThemeNameAsync();
                var colorCssFile = $"~/Themes/{workingThemeName}/Content/css/color.css";

                if (!File.Exists(_nopFileProvider.MapPath(colorCssFile)))
                {
                    await _logger.ErrorAsync("Color.css file missing.");
                }

                if (!_commonSettings.EnableCssBundling)
                {
                    var cssFileVersion = await _settingService.GetSettingByKeyAsync<int>($"{workingThemeName}themesettings.themeCustomCSSFileVersion", 0, _storeContext.GetCurrentStoreAsync().Id, false);
                    colorCssFile = colorCssFile + "?v=" + cssFileVersion;
                }

                return View("~/Plugins/NopStation.Theme.Arch/Views/Shared/_CustomCss.cshtml", colorCssFile);
            }

            return Content("");
        }
        #endregion
    }
}
