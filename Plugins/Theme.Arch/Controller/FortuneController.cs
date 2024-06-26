using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.NopStation.Theme.Arch.Controller
{
    public class ArchController : BasePublicController
    {
        #region Properties

        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IVendorService _vendorService;

        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly OrderSettings _orderSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IUserAgentHelper _userAgentHelper;

        #endregion

        #region Ctor

        public ArchController(IShoppingCartModelFactory shoppingCartModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
        
            IDiscountService discountService,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            IDateRangeService dateRangeService,

            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            OrderSettings orderSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            IProductModelFactory productModelFactory,
            IVendorService vendorService,
            IUserAgentHelper userAgentHelper)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _taxService = taxService;
            _currencyService = currencyService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            
            _discountService = discountService;
            _customerService = customerService;
            _giftCardService = giftCardService;
            _dateRangeService = dateRangeService;
        
            _workflowMessageService = workflowMessageService;
            _permissionService = permissionService;
            _downloadService = downloadService;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _productModelFactory = productModelFactory;

            _mediaSettings = mediaSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _orderSettings = orderSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;

            _vendorService = vendorService;
            _userAgentHelper = userAgentHelper;
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> ItemQuantityUpdate(IFormCollection form)
        {
            var cart =await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStoreAsync().Id);

            //get identifiers of items to remove
            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var products = (await _productService.GetProductsByIdsAsync(cart.Select(item => item.ProductId).Distinct().ToArray()))
                .ToDictionary(item => item.Id, item => item);

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item,
                Product = products.ContainsKey(item.ProductId) ? products[item.ProductId] : null
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = itemsWithNewQuantity
                .OrderByDescending(cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity &&
                     (cartItem.Product?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.Product != null && _shoppingCartService
                         .GetProductsRequiringProductAsync(cart, cartItem.Product).Any()))
                .ToList();

            //try to update cart items with new quantities and get warnings
            var warnings = orderedCart.Select(cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = _shoppingCartService.UpdateShoppingCartItemAsync(_workContext.GetCurrentCustomerAsync(),
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToList();

            //updated cart
            cart =await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStoreAsync().Id);

            //prepare model
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            var updatetopcartsectionhtml = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                                                    cart.Sum(ci => ci.Quantity));
            var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                                    ? await RenderViewComponentToStringAsync("FlyoutShoppingCart")
                                    : "";

            var miniModel =await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();
            var updatetopcartsubtotal = miniModel.SubTotal;


            if (warnings.Where(a => a.Warnings.Count != 0).Any())
            {
                return Json(new
                {
                    success = true,
                    message = string.Format(warnings.Where(a => a.Warnings.Count != 0).FirstOrDefault().Warnings.FirstOrDefault().ToString(), Url.RouteUrl("ShoppingCart")),
                    updatetopcartsectionhtml,
                    updateflyoutcartsectionhtml,
                    updatetopcartsubtotal
                });
            }

            return Json(new
            {
                success = true,
                message = string.Format(await _localizationService.GetResourceAsync("NopStation.Theme.Arch.ShoppingCart.Item.Quantity.Change.Notification"),
                Url.RouteUrl("ShoppingCart")),
                updatetopcartsectionhtml,
                updateflyoutcartsectionhtml,
                updatetopcartsubtotal
            });
        }
    }
}
