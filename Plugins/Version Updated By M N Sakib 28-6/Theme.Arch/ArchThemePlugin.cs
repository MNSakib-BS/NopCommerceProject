using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.NopStation.Theme.Arch.Models;
using Nop.Plugin.NopStation.Theme.Arch.Components;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Plugin.NopStation.Core;

namespace Nop.Plugin.NopStation.Theme.Arch
{
    public class ArchThemePlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INopStationCoreService _nopStationCoreService;

        #endregion

        #region Ctor

        public ArchThemePlugin(ISettingService settingService,
            IWebHelper webHelper,
            INopFileProvider nopFileProvider,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            INopStationCoreService nopStationCoreService)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _fileProvider = nopFileProvider;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _nopStationCoreService = nopStationCoreService;
        }

        #endregion

        #region Utilities

        private async Task CreateSampleDataAsync()
        {
            var sampleImagesPath = _fileProvider.MapPath("~/Plugins/NopStation.Theme.Arch/Content/sample/");

            var settings = new ArchThemeSettings()
            {
                EnableImageLazyLoad = true,
                FooterEmail = "estore@yourStore.com",
                LazyLoadPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "lazy-load.png")),
                    MimeTypes.ImagePng, "lazy-load")).Id,
                SupportedCardsPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-card-icons.png")),
                    MimeTypes.ImagePng, "footer-cards")).Id,
                LoginPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "login-image.jpg")),
                    MimeTypes.ImageJpeg, "login-image")).Id,
                FooterLogoPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-logo-white.png")),
                    MimeTypes.ImagePng, "footer-logo")).Id,
                ShowLogoAtPageFooter = true,
                ShowSupportedCardsPictureAtPageFooter = true,
                ShowLoginPictureAtLoginPage = true,
                DescriptionBoxOneTitle = "Support 24/7",
                DescriptionBoxOneText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxOnePictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "support-24-7.png")),
                    MimeTypes.ImagePng, "support-24-7")).Id,
                DescriptionBoxTwoTitle = "30 Day Return Policy",
                DescriptionBoxTwoText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxTwoPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "30-days-return-policy.png")),
                    MimeTypes.ImagePng, "30-days-return-policy")).Id,
                DescriptionBoxThreeTitle = "Worldwide Shipping",
                DescriptionBoxThreeText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxThreePictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "worldwide-shpping.png")),
                    MimeTypes.ImagePng, "worldwide-shpping")).Id,
                DescriptionBoxFourTitle = "Free Delivery",
                DescriptionBoxFourText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxFourPictureId = (await _pictureService.InsertPictureAsync(
                    await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "free-delivery-icon.png")),
                    MimeTypes.ImagePng, "free-delivery-icon")).Id,
            };
            await _settingService.SaveSettingAsync(settings);
        }

        #endregion

        #region Methods


        public Task<IList<string>> GetWidgetZonesAsync()
        {
            IList<string> widgetZones = new List<string>
            {
                PublicWidgetZones.HeadHtmlTag,
                ArchThemeDefaults.FooterDiscriptionWidgetZone,
                ArchThemeDefaults.CustomCssWidgetZone
            };
            return Task.FromResult(widgetZones);
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == ArchThemeDefaults.CustomCssWidgetZone)
            {
                return typeof(CustomCssViewComponent);
            }
            else if (widgetZone == ArchThemeDefaults.FooterDiscriptionWidgetZone)
            {
                return typeof(FooterTopDescriptionViewComponent);
            }
            return typeof(ArchViewComponent);
        }



        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ArchTheme/Configure";
        }

        public override async Task InstallAsync()
        {
            await CreateSampleDataAsync();
            await this.NopStationPluginInstallAsync(new ArchPermissionProvider());
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await this.NopStationPluginUninstallAsync(new ArchPermissionProvider());
            await base.UninstallAsync();
        }

        

       
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (await _permissionService.AuthorizeAsync(ArchPermissionProvider.ManageArch))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.Arch.Menu.Arch"),
                    Visible = true,
                    IconClass = "fa-circle-o",
                };

                var configItem = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.Arch.Menu.Configuration"),
                    Url = "/Admin/ArchTheme/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "Arch.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);

                await _nopStationCoreService.ManageSiteMapAsync(rootNode, menuItem, NopStationMenuType.Theme);
            }
        }
        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Menu.Arch", "Arch"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.CustomThemeColor", "Custom theme color"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.CustomThemeColor.Hint", "Choose a color that you want to site theme to display"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.EnableImageLazyLoad", "Enable image lazy-load"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.EnableImageLazyLoad.Hint", "Check to enable lazy-load for product box image."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.LazyLoadPictureId", "Lazy-load picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.LazyLoadPictureId.Hint", "This picture will be displayed initially in product box. Uploaded picture size should not be more than 4-5 KB."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.PanelTitle.DescriptionBoxOne", "Footer description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.PanelTitle.DescriptionBoxTwo", "Footer description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.PanelTitle.DescriptionBoxThree", "Footer description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.PanelTitle.DescriptionBoxFour", "Footer description box four"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.FooterEmail", "Footer email"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.FooterEmail.Hint", "Enter the email that you want to display on site footer."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowLoginPictureAtLoginPage", "Show login picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowLoginPictureAtLoginPage.Hint", "Check to show login picture at login page."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.LoginPicture", "Login picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.LoginPicture.Hint", "Login picture will be shown on the login page."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowLogoAtPageFooter", "Show footer logo"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowLogoAtPageFooter.Hint", "Check to show footer logo at footer."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.FooterLogoPicture", "Footer logo picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.FooterLogoPicture.Hint", "Footer logo picture will be shown on the page footer."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter", "Show supported cards picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter.Hint", "Check to show supported card picture at footer."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.SupportedCardsPicture", "Supported cards picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.SupportedCardsPicture.Hint", "This picture will be shown on page footer."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTitle", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTitle.Hint", "Enter title for footer description box one."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneText", "Text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneText.Hint", "Enter text for footer description box one."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOnePicture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOnePicture.Hint", "Picture will be shown at footer description box one."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTitle", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTitle.Hint", "Enter title for footer description box two."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoText", "Text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoText.Hint", "Enter text for footer description box two."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoPicture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoPicture.Hint", "Picture will be shown at footer description box two."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTitle", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTitle.Hint", "Enter title for footer description box three."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeText", "Text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeText.Hint", "Enter text for footer description box three."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreePicture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreePicture.Hint", "Picture will be shown at footer description box three."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTitle", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTitle.Hint", "Enter title for footer description box four."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourText", "Text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourText.Hint", "Enter text for footer description box four."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourPicture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourPicture.Hint", "Picture will be shown at footer description box four."));

            return list;
        }

 






        #endregion
    }
}
