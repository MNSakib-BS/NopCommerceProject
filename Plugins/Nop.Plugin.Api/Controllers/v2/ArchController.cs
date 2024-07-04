using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models.Arch;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/{version:apiVersion}/arch")]
    public class ArchController : SecureBaseApiController
    {
        private readonly IArchService _archService;
        private readonly ILogger _logger;

        public ArchController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ILogger logger,
            IArchService archService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _archService = archService;
            _logger = logger;
        }

        private bool ValidatePayloadChecksum(ArchRefundRequest request)
        {
            if (!Request.Headers.ContainsKey("PayloadChecksum"))
                return false;

            var computedChecksum = _archService.ComputePayloadChecksumAsync(request);
            
            var payloadChecksum = Request.Headers["PayloadChecksum"].ToString()?.ToLower();

            var isValid = computedChecksum.Result == payloadChecksum;
            if (!isValid)
                _logger.Error($"Invalid checksum {payloadChecksum} for request {request}");

            return isValid;
        }
        
        [HttpPost]
        [Route("refunds")]
        public async Task<IActionResult> Refunds([FromBody] ArchRefundRequest request)
        {
            var refundResponse =await _archService.RefundAsync(request, ValidatePayloadChecksum);
            return Json(refundResponse);
        }
    }
}