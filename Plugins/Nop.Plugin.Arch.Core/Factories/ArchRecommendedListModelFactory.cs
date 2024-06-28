using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Core.Factories
{
    /// <summary>
    /// Represents the topic model factory
    /// </summary>
    public partial class ArchRecommendedListModelFactory : RecommendedListModelFactory
    {
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICurrencyService _currencyService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxService _taxService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly bool _hasMultipleLanguages;

        public ArchRecommendedListModelFactory(
            IProductService productService,
            IWorkContext workContext,
            ICacheKeyService cacheKeyService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService)
            : base( productService,
             workContext,
             cacheKeyService,
             currencyService,
             localizationService,
             pictureService,
             priceFormatter,
             productAttributeFormatter,
             shoppingCartService,
             staticCacheManager,
             storeContext,
             taxService,
             urlRecordService,
             webHelper,
             mediaSettings,
             orderSettings,
             shoppingCartSettings,
             productAttributeParser,
             productAttributeService)
        {
            _productService = productService;
            _workContext = workContext;
            _cacheKeyService = cacheKeyService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _shoppingCartService = shoppingCartService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;

            _hasMultipleLanguages = EngineContext.Current.Resolve<ILanguageService>()?.GetAllLanguages()?.Count > 1;
        }

        public override RecommendedListModel.ShoppingCartItemModel PrepareRecommendedListItemModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var dictionary = new Dictionary<string, StringValues>();
            var mappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var mapping in mappings)
            {
                var attributeValue = _productAttributeService.GetProductAttributeValues(mapping.Id).FirstOrDefault(i => i.IsPreSelected);
                if (attributeValue != null)
                {
                    dictionary.Add($"product_attribute_{mapping.Id}", new StringValues(attributeValue.Id.ToString()));
                }
            }

            var productForm = new FormCollection(dictionary);
            var attributes = _productAttributeParser.ParseProductAttributes(product, productForm, new List<string>());

            var sci = new ShoppingCartItem
            {
                Id = product.Id,
                StoreId = _storeContext.CurrentStore.Id,
                Quantity = 1,
                ProductId = product.Id,
                CustomerId = _workContext.CurrentCustomer.Id,
                AttributesXml = attributes,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
            };

            var cartItemModel = new RecommendedListModel.ShoppingCartItemModel
            {
                Id = product.Id,
                ProductId = product.Id,
                ProductName = _hasMultipleLanguages ? _localizationService.GetLocalized(product, x => x.Name) : product.Name,
                ProductSeName = _urlRecordService.GetSeName(product),
                Quantity = sci.Quantity,
                AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml),
                Price = product.Price
            };

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //unit prices
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetUnitPrice(sci), out var _);
                var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                var shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(product, _shoppingCartService.GetSubTotal(sci, true, out var shoppingCartItemDiscountBase, out _, out var maximumDiscountQty), out _);
                var shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(product, shoppingCartItemDiscountBase, out _);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowProductImagesOnWishList)
            {
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                _workContext.CurrentCustomer,
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);


            return cartItemModel;
        }

        public override PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.CartPictureModelKey
                , sci, pictureSize, true, _workContext.WorkingLanguage, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore);

            var model = _staticCacheManager.Get(pictureCacheKey, () =>
            {
                var product = _productService.GetProductById(sci.ProductId);

                //shopping cart item picture
                var sciPicture = _pictureService.GetProductPicture(product, sci.AttributesXml);

                return new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(ref sciPicture, pictureSize, showDefaultPicture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });

            return model;
        }
    }
}