using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTOs.OrderNotes;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{
    public class OrderNotesController : SecureBaseApiController
    {
        private readonly IFactory<OrderNote> _factory;
        private readonly IOrderNoteService _orderNoteService;

        public OrderNotesController(
            IFactory<OrderNote> factory,
            IOrderNoteService orderNoteService,
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService, 
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService, 
            IPictureService pictureService) : 
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService, 
                storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _factory = factory;
            _orderNoteService = orderNoteService;
        }

        [HttpPost]
        [Route("/api/ordernotes")]
        [ProducesResponseType(typeof(OrderNotesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateOrderNote(
            [ModelBinder(typeof(JsonModelBinder<OrderNoteDto>))]
            Delta<OrderNoteDto> orderNoteDelta)
        {
            if (!ModelState.IsValid)
                return Error();

            var newOrderNote = await _factory.InitializeAsync();
            orderNoteDelta.Merge(newOrderNote);

            await _orderNoteService.InsertOrderNoteAsync(newOrderNote);

            var orderNotesRootObject = new OrderNotesRootObject();
            orderNotesRootObject.OrderNotes.Add(orderNoteDelta.Dto);

            var json = JsonFieldsSerializer.Serialize(orderNotesRootObject, string.Empty);
            return new RawJsonActionResult(json);
        }

    }
}
