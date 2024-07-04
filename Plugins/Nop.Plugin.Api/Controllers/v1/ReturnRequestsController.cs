using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.ReturnRequests;
using Nop.Plugin.Api.DTOs.ReturnRequests;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{
    public class ReturnRequestsController : SecureBaseApiController
    {
        private readonly IFactory<ReturnRequest> _factory;
        private readonly IReturnRequestService _returnRequestService;

        public ReturnRequestsController(
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService, 
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService, 
            IPictureService pictureService,
            IFactory<ReturnRequest> factory,
            IReturnRequestService returnRequestService) : 
            base(jsonFieldsSerializer, aclService, customerService, 
                storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _factory = factory;
            _returnRequestService = returnRequestService;
        }

        [HttpPost]
        [Route("/api/ReturnRequests")]
        [ProducesResponseType(typeof(ReturnRequestsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateOrderNote(
            [ModelBinder(typeof(JsonModelBinder<ReturnRequestDto>))]
            Delta<ReturnRequestDto> returnRequestDelta)
        {
            if (!ModelState.IsValid)
                return Error();

            var newReturnRequest = await _factory.InitializeAsync();
            returnRequestDelta.Merge(newReturnRequest);

            await _returnRequestService.InsertReturnRequestAsync(newReturnRequest);

            var returnRequestsRootObject = new ReturnRequestsRootObject();
            returnRequestsRootObject.ReturnRequests.Add(returnRequestDelta.Dto);

            var json = JsonFieldsSerializer.Serialize(returnRequestsRootObject, string.Empty);
            return new RawJsonActionResult(json);
        }
    }
}
