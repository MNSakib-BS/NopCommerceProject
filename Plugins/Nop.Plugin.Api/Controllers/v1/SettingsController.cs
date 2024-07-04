using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Mobile;
using Nop.Plugin.Api.DTO.ReturnRequests;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Plugin.Api.MappingExtensions;

namespace Nop.Plugin.Api.Controllers
{
    public class SettingsController : SecureBaseApiController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public SettingsController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IShippingService shippingService,
            IStoreContext storeContext,
            IPictureService pictureService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                   storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Returns mobile settings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/settings/mobile")]
        [ProducesResponseType(typeof(MobileSettingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetMobileSettings()
        {
            var storeId = _storeContext.GetCurrentStore().Id;
            var mobileSettings = _settingService.LoadSetting<MobileSettings>(storeId);

            var mobileSettingsDto = mobileSettings.ToDto();

            if (mobileSettings.LogoPictureId > 0)
            {
                var pictureBinary = PictureService.GetPictureBinaryByPictureId(mobileSettings.LogoPictureId);
                mobileSettingsDto.Logo = Convert.ToBase64String(pictureBinary.BinaryData);
            }

            var settingsRootObject = new MobileSettingsRootObject
            {
                MobileSettings = mobileSettingsDto
            };

            var json = JsonFieldsSerializer.Serialize(settingsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}
