using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTOs.OrderNotes;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Extensions;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    public class OrdersController : SecureBaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly ILogger _logger;
        private readonly IFactory<Order> _factory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IOrderNoteService _orderNoteService;
        private readonly IPaymentService _paymentService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShipmentService _shipmentService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IEventPublisher _eventPublisher;

        // We resolve the order settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private OrderSettings _orderSettings;

        public OrdersController(ILogger logger,
            IOrderApiService orderApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            IFactory<Order> factory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderNoteService orderNoteService,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IShippingService shippingService,
            IPictureService pictureService,
            IDTOHelper dtoHelper,
            IProductAttributeConverter productAttributeConverter,
            ICustomerApiService customerApiService,
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            IPriceFormatter priceFormatter,
            IShipmentService shipmentService,
            IPaymentPluginManager paymentPluginManager,
            IEventPublisher eventPublisher)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                   storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _logger = logger;
            _orderApiService = orderApiService;
            _factory = factory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderNoteService = orderNoteService;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _dtoHelper = dtoHelper;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
            _customerApiService = customerApiService;
            _paymentService = paymentService;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _customerService = customerService;
            _priceFormatter = priceFormatter;
            _shipmentService = shipmentService;
            _paymentPluginManager = paymentPluginManager;
            _eventPublisher = eventPublisher;
        }

        private OrderSettings OrderSettings => _orderSettings ?? (_orderSettings = EngineContext.Current.Resolve<OrderSettings>());

        /// <summary>
        ///     Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");
            }

            var storeId =_storeContext.GetCurrentStoreAsync().Id;

            var orders = await _orderApiService.GetOrdersAsync(parameters.Ids, parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.Limit, parameters.Page, parameters.SinceId,
                                                    parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                                                    parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = orders.SelectAwait(async x => await _dtoHelper.PrepareOrderDTOAsync(x)).ToList();

            var ordersRootObject = new OrdersRootObject
                                   {
                                       Orders = ordersAsDtos
                                   };

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);
           
            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/count")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrdersCount(OrdersCountParametersModel parameters)
        {
            var storeId =_storeContext.GetCurrentStoreAsync().Id;

            var ordersCount = await _orderApiService.GetOrdersCountAsync(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                                                              parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId, storeId);

            var ordersCountRootObject = new OrdersCountRootObject
                                        {
                                            Count = ordersCount
                                        };

            return Ok(ordersCountRootObject);
        }

        /// <summary>
        ///     Retrieve order by spcified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var order = await _orderApiService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var ordersRootObject = new OrdersRootObject();

            var orderDto = await _dtoHelper.PrepareOrderDTOAsync(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve all orders for customer
        /// </summary>
        /// <param name="customerId">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
        {
            IList<OrderDto> ordersForCustomer = await _orderApiService.GetOrdersByCustomerIdAsync(customerId)
                                                                      .Select(x => _dtoHelper.PrepareOrderDTO(x))
                                                                      .ToList();

            var ordersRootObject = new OrdersRootObject
                                   {
                                       Orders = ordersForCustomer
                                   };

            return Ok(ordersRootObject);
        }

        [HttpPost]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateOrder(
            [ModelBinder(typeof(JsonModelBinder<OrderDto>))]
            Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (orderDelta.Dto.CustomerId == null)
            {
                return Error();
            }

            // We doesn't have to check for value because this is done by the order validator.
            var customer = await CustomerService.GetCustomerByIdAsync(orderDelta.Dto.CustomerId.Value);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            var shippingRequired = false;

            if (orderDelta.Dto.OrderItems != null)
            {
                var shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStoreAsync().Id);
                if (shouldReturnError)
                {
                    return Error(HttpStatusCode.BadRequest);
                }

                shippingRequired = await IsShippingAddressRequired(orderDelta.Dto.OrderItems);
            }

            if (shippingRequired)
            {
                var isValid = true;

                isValid &= await SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                                             orderDelta.Dto.ShippingMethod,
                                             orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStoreAsync().Id,
                                             customer,
                                             BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItems.ToList(),
                                                                                     customer.Id,
                                                                                     orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStoreAsync().Id));

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            var newOrder = await _factory.InitializeAsync();
            orderDelta.Merge(newOrder);

            customer.BillingAddressId = newOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
            customer.ShippingAddressId = newOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress.Id;
                       

            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.CustomerId = customer.Id;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue)
            {
                newOrder.StoreId = _storeContext.GetCurrentStoreAsync().Id;
            }

            var placeOrderResult = PlaceOrder(newOrder, customer);

            if (!placeOrderResult.IsCompletedSuccessfully)
            {
                foreach (var error in placeOrderResult.Errors)
                {
                    ModelState.AddModelError("order placement", error);
                }

                return Error(HttpStatusCode.BadRequest);
            }

            await CustomerActivityService.InsertActivityAsync("AddNewOrder",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.AddNewOrder"), newOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(placeOrderResult.PlacedOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var orderToDelete = await _orderApiService.GetOrderByIdAsync(id);

            if (orderToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            await _orderProcessingService.DeleteOrderAsync(orderToDelete);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteOrder", await LocalizationService.GetResourceAsync("ActivityLog.DeleteOrder"), orderToDelete);

            return new RawJsonActionResult("{}");
        }

        [HttpPut]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateOrder(
            [ModelBinder(typeof(JsonModelBinder<OrderDto>))]
            Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var currentOrder = await _orderApiService.GetOrderByIdAsync(orderDelta.Dto.Id);

            if (currentOrder == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var customer = await CustomerService.GetCustomerByIdAsync(currentOrder.CustomerId);
            
            var shippingRequired = await _orderService.GetOrderItemsAsync(currentOrder.Id)
                                                      .Any(item => !_productService.GetProductById(item.Id).IsFreeShipping);

            if (shippingRequired)
            {
                var isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    var storeId = orderDelta.Dto.StoreId ??  _storeContext.GetCurrentStoreAsync().Id;

                    isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                                                 orderDelta.Dto.ShippingMethod,
                                                 storeId,
                                                 customer, BuildShoppingCartItemsFromOrderItems(await _orderService.GetOrderItemsAsync(currentOrder.Id).ToList(), customer.Id, storeId));
                }

                if (isValid)
                {
                    currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
                }
                else
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            orderDelta.Merge(currentOrder);

            customer.BillingAddressId = currentOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
            customer.ShippingAddressId = currentOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress.Id;


            await _orderService.UpdateOrderAsync(currentOrder);

            await CustomerActivityService.InsertActivityAsync("UpdateOrder",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.UpdateOrder"), currentOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private async Task<bool> SetShippingOption(
            string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Customer customer, List<ShoppingCartItem> shoppingCartItems)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_rate_computation_method_system_name",
                                         "Please provide shipping_rate_computation_method_system_name");
            }
            else if (string.IsNullOrEmpty(shippingOptionName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
            }
            else
            {
                var shippingOptionResponse = await _shippingService.GetShippingOptionsAsync(shoppingCartItems, await CustomerService.GetCustomerShippingAddressAsync(customer), customer,
                                                                                 shippingRateComputationMethodSystemName, storeId);

                if (shippingOptionResponse.Success)
                {
                    var shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    var shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));

                    await _genericAttributeService.SaveAttributeAsync(customer,
                                                           NopCustomerDefaults.SelectedShippingOptionAttribute,
                                                           shippingOption, storeId);
                }
                else
                {
                    isValid = false;

                    foreach (var errorMessage in shippingOptionResponse.Errors)
                    {
                        ModelState.AddModelError("shipping_option", errorMessage);
                    }
                }
            }

            return isValid;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItems)
            {
                shoppingCartItems.Add(new ShoppingCartItem
                                      {
                                          ProductId = orderItem.ProductId,
                                          CustomerId = customerId,
                                          Quantity = orderItem.Quantity,
                                          RentalStartDateUtc = orderItem.RentalStartDateUtc,
                                          RentalEndDateUtc = orderItem.RentalEndDateUtc,
                                          StoreId = storeId,
                                          ShoppingCartType = ShoppingCartType.ShoppingCart
                                      });
            }

            return shoppingCartItems;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItemDtos)
            {
                if (orderItem.ProductId != null)
                {
                    shoppingCartItems.Add(new ShoppingCartItem
                                          {
                                              ProductId = orderItem.ProductId.Value, // required field
                                              CustomerId = customerId,
                                              Quantity = orderItem.Quantity ?? 1,
                                              RentalStartDateUtc = orderItem.RentalStartDateUtc,
                                              RentalEndDateUtc = orderItem.RentalEndDateUtc,
                                              StoreId = storeId,
                                              ShoppingCartType = ShoppingCartType.ShoppingCart
                                          });
                }
            }

            return shoppingCartItems;
        }

        private async Task<PlaceOrderResult> PlaceOrder(Order newOrder, Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest
                                        {
                                            StoreId = newOrder.StoreId,
                                            CustomerId = customer.Id,
                                            PaymentMethodSystemName = newOrder.PaymentMethodSystemName
                                        };


            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);

            return placeOrderResult;
        }

        private async Task<bool> IsShippingAddressRequired(ICollection<OrderItemDto> orderItems)
        {
            var shippingAddressRequired = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

                    shippingAddressRequired |= product.IsShipEnabled;
                }
            }

            return shippingAddressRequired;
        }

        private async Task<bool> AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
        {
            var shouldReturnError = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

                    if (!product.IsRental)
                    {
                        orderItem.RentalStartDateUtc = null;
                        orderItem.RentalEndDateUtc = null;
                    }

                    var attributesXml = await _productAttributeConverter.ConvertToXmlAsync(orderItem.Attributes.ToList(), product.Id);

                    var errors = await _shoppingCartService.AddToCartAsync(customer, product,
                                                                ShoppingCartType.ShoppingCart, storeId, attributesXml,
                                                                0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                                                                orderItem.Quantity ?? 1);

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("order", error);
                        }

                        shouldReturnError = true;
                    }
                }
            }

            return shouldReturnError;
        }

        [HttpPost]
        [Route("/api/orders/GetOrdersForDriver")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetOrdersForDriver(
            [ModelBinder(typeof(JsonModelBinder<OrderSearchDto>))]
            Delta<OrderSearchDto> orderSearchDelta)
        {           
            if (!orderSearchDelta.Dto.DriverId.HasValue && string.IsNullOrWhiteSpace(orderSearchDelta.Dto.DriverEmail))
                ModelState.AddModelError(string.Empty, "Driver Id or email is required");

            if (!ModelState.IsValid)
                return Error();

            var searchDto = orderSearchDelta.Dto;

            int? driverId = searchDto.DriverId.HasValue ? searchDto.DriverId : ( _customerApiService.GetCustomerByEmailAsync(searchDto.DriverEmail)?.Id);

            if (driverId == null)
            {
                ModelState.AddModelError(string.Empty, "Driver not found");
                return Error();
            }

            var storeIds = GetStoreIdsForDriver(driverId.Value);
          
            var orderDtos = GetOrdersForStores(storeIds, searchDto);
          
            var ordersRootObject = new OrdersRootObject() { Orders = orderDtos };
            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/orders/GetOrdersForDriverSummary")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetOrdersForDriverSummary(
            [ModelBinder(typeof(JsonModelBinder<OrderSearchDto>))]
            Delta<OrderSearchDto> orderSearchDelta)
        {
            if (!orderSearchDelta.Dto.DriverId.HasValue && string.IsNullOrWhiteSpace(orderSearchDelta.Dto.DriverEmail))
                ModelState.AddModelError(string.Empty, "Driver Id or email is required");

            if (!ModelState.IsValid)
                return Error();

            var searchDto = orderSearchDelta.Dto;
            int? driverId = searchDto.DriverId.HasValue ? searchDto.DriverId : (_customerApiService.GetCustomerByEmailAsync(searchDto.DriverEmail)?.Id);

            if (driverId == null)
            {
                ModelState.AddModelError(string.Empty, "Driver not found");
                return Error();
            }

            var storeIds = GetStoreIdsForDriver(driverId.Value);

            var summary = new OrdersForDriverSummaryDto();

            // New Orders
            searchDto.DriverId = null; // ensure searchDto does not have a driverId at this point (new orders are unallocated)
            searchDto.ShippingStatusIds = new int[] { (int)ShippingStatus.ReadyForCollection }; // search for new orders
            var newOrders = GetOrdersForStores(storeIds, searchDto);
            
            summary.NewOrdersCount = newOrders.Count();
            summary.NewOrdersWithChangesCount = newOrders.Count(i => i.OrderNotes.Any());

            // Orders To Be Collected
            searchDto.DriverId = driverId; // ensure searchDto has driverId at this point
            if (searchDto.OrderStatusIds == null)
            {
                searchDto.OrderStatusIds = new int[] { (int)OrderStatus.Processing };
            }
            if (!searchDto.OrderStatusIds.Contains((int)OrderStatus.Cancelled))
            {
                searchDto.OrderStatusIds.Add((int)OrderStatus.Cancelled);
            }
            var ordersForCollection = GetOrdersForStores(storeIds, searchDto);
            
            summary.OrdersForCollectionCount = ordersForCollection.Count();
            summary.OrdersForCollectionWithChangesCount = ordersForCollection.Count(i => i.OrderNotes.Any());

            // Orders To Be Delivered
            searchDto.ShippingStatusIds = new int[] { (int)ShippingStatus.Shipped }; // search for to be delivered
            var ordersToBeDelivered = GetOrdersForStores(storeIds, searchDto);

            summary.OrdersToBeDeliveredCount = ordersToBeDelivered.Count();
            summary.OrdersToBeDeliveredWithChangesCount = ordersToBeDelivered.Count(i => i.OrderNotes.Any());

            // Orders With Returns
            searchDto.ShippingStatusIds = new int[] { (int)ShippingStatus.DeliveredWithReturns }; // search for returns
            var ordersWithReturns = GetOrdersForStores(storeIds, searchDto);
            summary.OrdersWithReturnsCount = ordersWithReturns.Count();

            return Ok(new OrdersSummaryRootObject() { Summary = summary });
        }

        private async Task<List<int>> GetStoreIdsForDriver(int driverId)
        {
            var customer = await CustomerService.GetCustomerByIdAsync(driverId);
            var storeMappings = await StoreMappingService.GetStoreMappingsAsync(customer);
            return storeMappings?.Select(i => i.StoreId)?.ToList();
        }

        private async Task<IList<OrderDto>> GetOrdersForStores(List<int> storeIds, OrderSearchDto searchDto)
        {
            var orders = await _orderApiService.GetOrdersForStoresAsync(storeIds, searchDto.CreatedAtMin, searchDto.CreatedAtMax,
                                                             searchDto.Limit ?? Constants.Configurations.DefaultLimit,
                                                             searchDto.Page ?? Constants.Configurations.DefaultPageValue,
                                                             searchDto.OrderStatusIds, searchDto.PaymentStatusIds, searchDto.ShippingStatusIds,
                                                             searchDto.DriverId, DateTime.UtcNow.EndOfDay());

            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderDto = await _dtoHelper.PrepareOrderDTOAsync(order);

                var orderNotes =await  _orderNoteService.GetOrderNotesAsync(order.Id,
                                                                 orderNoteTypeId: searchDto.OrderNoteType,
                                                                 includeAcknowledged: searchDto.IncludeAcknowledgedNotes.HasValue
                                                                 ? searchDto.IncludeAcknowledgedNotes.Value : true);
                
                orderDto.OrderNotes = new List<OrderNoteDto>();

                foreach (var orderNote in orderNotes)
                {
                    var orderNoteDto =await _dtoHelper.PrepareOrderNoteDtoAsync(orderNote);

                    orderDto.OrderNotes.Add(orderNoteDto);
                }

                orderDtos.Add(orderDto);
            }

            return orderDtos;
        }

        [HttpPost]
        [Route("/api/orders/UpdateStatus")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateOrderStatus(
            [ModelBinder(typeof(JsonModelBinder<OrderStatusUpdateDto>))]
            Delta<OrderStatusUpdateDto> orderStatusUpdateDelta)
        {
            if (!ModelState.IsValid)
                return Error();

            var order = await _orderApiService.GetOrderByIdAsync(orderStatusUpdateDelta.Dto.Id);
            if (order == null)
                return Error(HttpStatusCode.NotFound, "order", "not found");

            var orderStatusUpdate = orderStatusUpdateDelta.Dto;

            var isAllocatedToDriverIdChanged = order.AllocatedToDriverId != orderStatusUpdate.AllocatedToDriverId;

            if (isAllocatedToDriverIdChanged)
            {
                order.AllocatedToDriverId = orderStatusUpdate.AllocatedToDriverId;

                await _orderService.UpdateOrderAsync(order);
                var note = orderStatusUpdate.AllocatedToDriverId.HasValue ? $"Order has been allocated to driver {order.AllocatedToDriverId}"
                                                                          : $"Order has been deallocated from driver {order.AllocatedToDriverId}";

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = note,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    CreatedByCustomerId = order.AllocatedToDriverId
                });
            }

            if (orderStatusUpdate.ShippingStatus.HasValue)
            {
                switch (orderStatusUpdate.ShippingStatus)
                {
                    case ShippingStatus.ReadyForCollection:
                        UpdateToReadyForCollection(order);
                        break;
                    case ShippingStatus.Shipped:
                        _orderProcessingService.CreateShipment(order);
                        break;
                    case ShippingStatus.DeliveredWithReturns:
                        DeliverWithReturns(order);
                        break;
                    case ShippingStatus.Delivered:
                        DeliverOrder(order);
                        break;
                    case ShippingStatus.ReturnedToStore:
                        ReturnToStore(order);
                        break;
                    default:
                        break;
                }
            }

            await CustomerActivityService.InsertActivityAsync("UpdateOrder",
                                                  await LocalizationService.GetResourceAsync("ActivityLog.UpdateOrder"), order);

            var ordersRootObject = new OrdersRootObject();
            var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(order);
            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);
            return new RawJsonActionResult(json);
        }

        private void UpdateToReadyForCollection(Order order)
        {
            var shipment = GetLatestShipment(order, false);
            _orderProcessingService.UpdateToReadyForCollection(order, shipment);
        }

        private void DeliverWithReturns(Order order)
        {
            var shipment = GetLatestShipment(order, false);
            _orderProcessingService.DeliverWithReturns(shipment, true);
        }

        private async Task DeliverOrder(Order order)
        {
            var shipment = GetLatestShipment(order, false);
            await _orderProcessingService.DeliverAsync(shipment, true);
        }

        private void ReturnToStore(Order order)
        {
            var shipment = GetLatestShipment(order, true);
            _orderProcessingService.CompleteDeliveryWithReturns(shipment);
        }

        private async Task<Shipment> GetLatestShipment(Order order, bool isDelivered, bool isReturned = false)
        {
            return await _shipmentService.GetShipmentsByOrderIdAsync(order.Id, shipped: true)
                                   .Where(i => i.ReturnedDateUtc.HasValue == isReturned && 
                                               i.DeliveryDateUtc.HasValue == isDelivered)
                                   .OrderByDescending(i => i.DeliveryDateUtc)
                                   .FirstOrDefault();
        }
    }
}
