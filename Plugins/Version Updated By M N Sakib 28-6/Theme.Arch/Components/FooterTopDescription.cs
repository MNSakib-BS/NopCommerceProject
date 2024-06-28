using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.Theme.Arch.Infrastructure.Cache;
using Nop.Plugin.NopStation.Theme.Arch.Models;
using Nop.Services.Caching;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.Arch.Components
{
    public class FooterTopDescriptionViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ArchThemeSettings _ArchSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public FooterTopDescriptionViewComponent(
            IWorkContext workContext,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ArchThemeSettings ArchSettings,
            ILocalizationService localizationService,
            IPictureService pictureService)
        {
            _workContext = workContext;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _ArchSettings = ArchSettings;
            _localizationService = localizationService;
            _pictureService = pictureService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (
               !_ArchSettings.EnableDescriptionBoxOne
            && !_ArchSettings.EnableDescriptionBoxTwo
            && !_ArchSettings.EnableDescriptionBoxThree
            && !_ArchSettings.EnableDescriptionBoxFour)
            {
                return Content("");
            }

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.FOOTER_DESCRIPTION_MODEL_KEY, storeId, languageId);

            var descriptionBoxes = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var model = new List<FooterTopDescriptionModel>();

                if (_ArchSettings.EnableDescriptionBoxOne)
                {
                    var title = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTitleValue");
                    var text = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxOneTextValue");
                    var url = _ArchSettings.DescriptionBoxOneUrl;
                    var pictureUrl = await _pictureService.GetPictureUrlAsync(_ArchSettings.DescriptionBoxOnePictureId);

                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }

                if (_ArchSettings.EnableDescriptionBoxTwo)
                {
                    var title = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTitleValue");
                    var text = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxTwoTextValue");
                    var url = _ArchSettings.DescriptionBoxTwoUrl;
                    var pictureUrl = await _pictureService.GetPictureUrlAsync(_ArchSettings.DescriptionBoxTwoPictureId);

                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }

                if (_ArchSettings.EnableDescriptionBoxThree)
                {
                    var title = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTitleValue");
                    var text = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxThreeTextValue");
                    var url = _ArchSettings.DescriptionBoxThreeUrl;
                    var pictureUrl = await _pictureService.GetPictureUrlAsync(_ArchSettings.DescriptionBoxThreePictureId);

                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }

                if (_ArchSettings.EnableDescriptionBoxFour)
                {
                    var title = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTitleValue");
                    var text = await GetResourceForCurrentStoreAsync("Admin.NopStation.Theme.Arch.Configuration.Fields.DescriptionBoxFourTextValue");
                    var url = _ArchSettings.DescriptionBoxFourUrl;
                    var pictureUrl = await _pictureService.GetPictureUrlAsync(_ArchSettings.DescriptionBoxFourPictureId);

                    model.Add(new FooterTopDescriptionModel
                    {
                        Title = title,
                        Text = text,
                        Url = url,
                        PictureUrl = pictureUrl
                    });
                }

                return model;
            });

            return View("~/Plugins/NopStation.Theme.Arch/Views/_FooterTopDescription.cshtml", descriptionBoxes);
        }

        private async Task<string> GetResourceForCurrentStoreAsync(string resourceName)
        {
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            var resourceKey = $"{resourceName}-{storeId}";
            var resourceValue = await _localizationService.GetResourceAsync(resourceKey, languageId, false, "", true);

            if (!string.IsNullOrEmpty(resourceValue))
            {
                return resourceValue;
            }

            return await _localizationService.GetResourceAsync(resourceName, languageId, false, "", true);
        }

        #endregion
    }
}
