using System.Net.Http.Headers;
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

namespace Nop.Plugin.Api.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("/arch")]
    public class ArchController: BaseApiController
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
            if (!Request.Headers.ContainsKey("Authorization"))
                return false;

            var computedChecksum = _archService.ComputePayloadChecksumAsync(request);

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var payloadChecksum = authHeader.Parameter?.ToLower();

            var isValid = computedChecksum.Result == payloadChecksum;
            if (!isValid)
                _logger.Error($"Invalid checksum {payloadChecksum} for request {request}");

            return isValid;
        }
        
        [HttpPost]
        [Route("refunds")]
        public IActionResult Refunds([FromBody] ArchRefundRequest request)
        {
            var refundResponse = _archService.RefundAsync(request, ValidatePayloadChecksum);
            return Json(refundResponse);
        }
    }
}