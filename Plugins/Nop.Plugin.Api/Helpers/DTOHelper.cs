using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTO.Categories;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Languages;
using Nop.Plugin.Api.DTO.Manufacturers;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTO.ProductAttributes;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.DTO.Stores;
using Nop.Plugin.Api.DTOs.OrderNotes;
using Nop.Plugin.Api.DTOs.ReturnRequests;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using NopCustomerDefaults = Nop.Plugin.Arch.Core.Domains.Customers.NopCustomerDefaults;

namespace Nop.Plugin.Api.Helpers
{
    public class DTOHelper : IDTOHelper
    {
        private readonly IAclService _aclService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IDiscountService _discountService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productPictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressService _addressService;
        private readonly IOrderNoteService _orderNoteService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IResanehlabService _resanehlabService;
        private readonly bool _hasMultipleLanguages;

        public DTOHelper(
            IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ICustomerService customerApiService,
            IProductAttributeParser productAttributeParser,
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IStoreService storeService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IProductTagService productTagService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IProductAttributeConverter productAttributeConverter,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IAddressService addressService,
            IReturnRequestService returnRequestService,
            ICacheKeyService cacheKeyService,
            IResanehlabService resanehlabService)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _customerService = customerApiService;
            _productAttributeParser = productAttributeParser;
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _storeService = storeService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _productAttributeConverter = productAttributeConverter;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _addressService = addressService;
            _returnRequestService = returnRequestService;
            _cacheKeyService = cacheKeyService;
            _resanehlabService = resanehlabService;

            _hasMultipleLanguages = EngineContext.Current.Resolve<ILanguageService>()?.GetAllLanguages()?.Count > 1;
        }

        public async Task<ProductDto> PrepareProductDTOAsync(Product product)
        {
            if(product == null) return null;
            var productDto = product.ToDto();
            var productPictures =await _productService.GetProductPicturesByProductIdAsync(product.Id);
            PrepareProductImagesAsync(productPictures, productDto);

            productDto.SeName =await _urlRecordService.GetSeNameAsync(product);
            productDto.DiscountIds =(await _discountService.GetAppliedDiscountsAsync(product)).Select(discount => discount.Id).ToList();
            productDto.ManufacturerIds =(await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).Select(pm => pm.Id).ToList();
            productDto.RoleIds = (await _aclService.GetAclRecordsAsync(product)).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds =( await _storeMappingService.GetStoreMappingsAsync(product)).Select(mapping => mapping.StoreId)
                                                      .ToList();
            productDto.Tags =( await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name)
                                                .ToList();

            productDto.AssociatedProductIds =
              (await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true))
                               .Select(associatedProduct => associatedProduct.Id)
                               .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            productDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _hasMultipleLanguages ?await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id) : product.Name
                                       };

                productDto.LocalizedNames.Add(localizedNameDto);
            }

            return productDto;
        }

        public async Task<CategoryDto> PrepareCategoryDTOAsync(Category category)
        {
            var categoryDto = category.ToDto();

            var picture =await _pictureService.GetPictureByIdAsync(category.PictureId);
            var pictureCategory =await _pictureService.GetPictureByIdAsync(category.BannerHeroPictureTopId);
            var pictureCategoryLower =await _pictureService.GetPictureByIdAsync(category.BannerHeroPictureLowerId);

            var imageDto = PrepareImageDto(picture);
            var imageCategoryDto = PrepareImageDto(pictureCategory);
            var imageCategoryLowerDto = PrepareImageDto(pictureCategoryLower);

            if (imageDto != null)
            {
                categoryDto.Image = imageDto;
            }

            if (imageCategoryDto != null)
            {
                categoryDto.BannerHeroPictureTop = imageCategoryDto;
            }

            if (imageCategoryLowerDto != null)
            {
                categoryDto.BannerHeroPictureLower = imageCategoryDto;
            }

            categoryDto.SeName =await _urlRecordService.GetSeNameAsync(category);
            categoryDto.DiscountIds =(await _discountService.GetAppliedDiscountsAsync(category)).Select(discount => discount.Id).ToList();
            categoryDto.RoleIds =(await _aclService.GetAclRecordsAsync(category)).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds =( await _storeMappingService.GetStoreMappingsAsync(category)).Select(mapping => mapping.StoreId)
                                                       .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            categoryDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _hasMultipleLanguages ? await _localizationService.GetLocalizedAsync(category, x => x.Name, language.Id) : category.Name
                                       };

                categoryDto.LocalizedNames.Add(localizedNameDto);
            }

            return categoryDto;
        }

        public async Task< OrderDto> PrepareOrderDTOAsync(Order order)
        {
            var orderDto = order.ToDto();

            orderDto.OrderItems =(await _orderService.GetOrderItemsAsync(order.Id))
                                    .SelectAwait(s=> PrepareOrderItemDTOAsync(s,order.StoreId))
                                    .ToList();

            orderDto.ReturnRequests = new List<ReturnRequestDto>();

            foreach (var item in orderDto.OrderItems)
            {
                var returnRequests =await _returnRequestService.SearchReturnRequestsAsync(orderItemId: item.Id);

                if (returnRequests != null)
                {
                    foreach (var request in returnRequests)
                    {
                        orderDto.ReturnRequests.Add(request.ToDto());
                    }
                }
            }

            var customer =await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer != null)
            {
                orderDto.Customer = customer.ToOrderCustomerDto();

                var attributes =await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, nameof(Customer));

                orderDto.Customer.FirstName = attributes.FirstOrDefault(i => i.Key.Equals(NopCustomerDefaults.FirstNameAttribute))?.Value;
                orderDto.Customer.LastName = attributes.FirstOrDefault(i => i.Key.Equals(NopCustomerDefaults.LastNameAttribute))?.Value;

                if (orderDto.StoreId.HasValue)
                    orderDto.Store =(await _storeService.GetStoreByIdAsync(orderDto.StoreId.Value))?.ToDto();
            }

            if (order.ShippingAddressId.HasValue)
                orderDto.ShippingAddress =(await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value))?.ToDto();
            
            orderDto.OneTimePin =await _genericAttributeService.GetAttributeAsync<string>(order, _cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.OrderOneTimePinCacheKey, order.Id).Key, order.StoreId);
            orderDto.DistanceFromStoreToShippingAddress = await _genericAttributeService.GetAttributeAsync<string>(order, _cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.DistanceFromStoreToShippingAddressKey, order.Id).Key, order.StoreId);
            orderDto.EstimatedDeliveryDate = _resanehlabService.GetEstimatedDateForOrder(order.Id);
            orderDto.AdminShippingStatusUpdateId = await _genericAttributeService.GetAttributeAsync<int>(order, _cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.AdminShippingStatusUpdateCacheKey, order.Id).Key, order.StoreId);

            return orderDto;
        }

        public async Task <OrderNoteDto> PrepareOrderNoteDtoAsync(OrderNote orderNote)
        {
            var dto = orderNote.ToDto();           
            dto.OrderNoteChangeTypeId = await _genericAttributeService.GetAttributeAsync<int?>(orderNote, _cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.OrderNoteChangeTypeCacheKey, orderNote.Id).Key);

            return dto;
        }

        public async Task<ShoppingCartItemDto> PrepareShoppingCartItemDTOAsync(ShoppingCartItem shoppingCartItem)
        {
            var dto = shoppingCartItem.ToDto();
            dto.ProductDto =await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(shoppingCartItem.ProductId));
            dto.CustomerDto =(await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId)).ToCustomerForShoppingCartItemDto();
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }

        public async Task <OrderItemDto> PrepareOrderItemDTOAsync(OrderItem orderItem, int storeId = 0)
        {
            var dto = orderItem.ToDto();
            dto.Product =await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(orderItem.ProductId));

            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }

        public async Task <OrderItemDto> PrepareOrderItemDTOAsync(OrderItem orderItem)
        {
            return await PrepareOrderItemDTOAsync(orderItem, storeId: 1);
        }

        public async Task<StoreDto> PrepareStoreDTOAsync(Store store)
        {
            var storeDto = store.ToDto();

            var primaryCurrency =await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale))
            {
                storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;
            }

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }

        public async Task<LanguageDto> PrepareLanguageDtoAsync(Language language)
        {
            var languageDto = language.ToDto();

            languageDto.StoreIds =(await _storeMappingService.GetStoreMappingsAsync(language)).Select(mapping => mapping.StoreId)
                                                       .ToList();

            if (languageDto.StoreIds.Count == 0)
            {
                languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();
            }

            return languageDto;
        }

        public async Task<ProductAttributeDto> PrepareProductAttributeDTOAsync(ProductAttribute productAttribute)
        {
            return productAttribute.ToDto();
        }

        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDtoAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.ToDto();
        }

        public async Task <SpecificationAttributeDto> PrepareSpecificationAttributeDtoAsync(SpecificationAttribute specificationAttribute)
        {
            return specificationAttribute.ToDto();
        }

        public async Task<ManufacturerDto> PrepareManufacturerDtoAsync(Manufacturer manufacturer)
        {
            var manufacturerDto = manufacturer.ToDto();

            var picture =await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
            var imageDto = PrepareImageDto(picture);

            if (imageDto != null)
            {
                manufacturerDto.Image = imageDto;
            }

            manufacturerDto.SeName =await _urlRecordService.GetSeNameAsync(manufacturer);
            manufacturerDto.DiscountIds =(await _discountService.GetAppliedDiscountsAsync(manufacturer)).Select(discount => discount.Id).ToList();
            manufacturerDto.RoleIds =(await _aclService.GetAclRecordsAsync(manufacturer)).Select(acl => acl.CustomerRoleId).ToList();
            manufacturerDto.StoreIds =(await _storeMappingService.GetStoreMappingsAsync(manufacturer)).Select(mapping => mapping.StoreId)
                                                           .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            manufacturerDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _hasMultipleLanguages ?await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name, language.Id) : manufacturer.Name
                                       };

                manufacturerDto.LocalizedNames.Add(localizedNameDto);
            }

            return manufacturerDto;
        }

        private async Task PrepareProductImagesAsync(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null)
            {
                productDto.Images = new List<ImageMappingDto>();
            }

            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                var imageDto =PrepareImageDto(await _pictureService.GetPictureByIdAsync(productPicture.PictureId));

                if (imageDto != null)
                {
                    var productImageDto = new ImageMappingDto
                                          {
                                              Id = productPicture.Id,
                                              PictureId = productPicture.PictureId,
                                              Position = productPicture.DisplayOrder,
                                              Src = imageDto.Src,
                                              Attachment = imageDto.Attachment
                                          };

                    productDto.Images.Add(productImageDto);
                }
            }
        }

        protected ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                image = new ImageDto
                        {
                            //Attachment = Convert.ToBase64String(picture.PictureBinary),
                            //Src = _pictureService.GetPictureUrl(picture)
                        };
            }

            return image;
        }

        private void PrepareProductAttributes(
            IEnumerable<ProductAttributeMapping> productAttributeMappings,
            ProductDto productDto)
        {
            if (productDto.ProductAttributeMappings == null)
            {
                productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();
            }

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                var productAttributeMappingDto =
                    PrepareProductAttributeMappingDto(productAttributeMapping);

                if (productAttributeMappingDto != null)
                {
                    productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
                }
            }
        }

        private ProductAttributeMappingDto PrepareProductAttributeMappingDto(
            ProductAttributeMapping productAttributeMapping)
        {
            ProductAttributeMappingDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
            {
                productAttributeMappingDto = new ProductAttributeMappingDto
                                             {
                                                 Id = productAttributeMapping.Id,
                                                 ProductAttributeId = productAttributeMapping.ProductAttributeId,
                                                 ProductAttributeName = _productAttributeService
                                                                        .GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId).Name,
                                                 TextPrompt = productAttributeMapping.TextPrompt,
                                                 DefaultValue = productAttributeMapping.DefaultValue,
                                                 AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                                                 DisplayOrder = productAttributeMapping.DisplayOrder,
                                                 IsRequired = productAttributeMapping.IsRequired,
                                                 //TODO: Somnath
                                                 //ProductAttributeValues = _productAttributeService.GetProductAttributeValueById(productAttributeMapping.Id).
                                                 //                                                .Select(x =>
                                                 //                                                            PrepareProductAttributeValueDto(x,
                                                 //                                                                                            productAttributeMapping
                                                 //                                                                                                .Product))
                                                 //                                                .ToList()
                                             };
            }

            return productAttributeMappingDto;
        }

        private async Task<ProductAttributeValueDto> PrepareProductAttributeValueDto(
            ProductAttributeValue productAttributeValue,
            Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue != null)
            {
                productAttributeValueDto = productAttributeValue.ToDto();
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    var imageSquaresPicture =
                        await _pictureService.GetPictureByIdAsync(productAttributeValue.ImageSquaresPictureId);
                    var imageDto = PrepareImageDto(imageSquaresPicture);
                    productAttributeValueDto.ImageSquaresImage = imageDto;
                }

                if (productAttributeValue.PictureId > 0)
                {
                    // make sure that the picture is mapped to the product
                    // This is needed since if you delete the product picture mapping from the nopCommerce administrationthe
                    // then the attribute value is not updated and it will point to a picture that has been deleted
                    var productPicture =
                     (await _productPictureService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
                    if (productPicture != null)
                    {
                        productAttributeValueDto.ProductPictureId = productPicture.Id;
                    }
                }
            }

            return productAttributeValueDto;
        }

        private void PrepareProductAttributeCombinations(
            IEnumerable<ProductAttributeCombination> productAttributeCombinations,
            ProductDto productDto)
        {
            productDto.ProductAttributeCombinations = productDto.ProductAttributeCombinations ?? new List<ProductAttributeCombinationDto>();

            foreach (var productAttributeCombination in productAttributeCombinations)
            {
                var productAttributeCombinationDto = PrepareProductAttributeCombinationDto(productAttributeCombination);
                if (productAttributeCombinationDto != null)
                {
                    productDto.ProductAttributeCombinations.Add(productAttributeCombinationDto);
                }
            }
        }

        private ProductAttributeCombinationDto PrepareProductAttributeCombinationDto(ProductAttributeCombination productAttributeCombination)
        {
            return productAttributeCombination.ToDto();
        }

        public void PrepareProductSpecificationAttributes(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
        {
            if (productDto.ProductSpecificationAttributes == null)
            {
                productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();
            }

            foreach (var productSpecificationAttribute in productSpecificationAttributes)
            {
                var productSpecificationAttributeDto = PrepareProductSpecificationAttributeDtoAsync(productSpecificationAttribute);

                if (productSpecificationAttributeDto != null)
                {
                    productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
                }
            }
        }

        Task<ProductSpecificationAttributeDto> IDTOHelper.PrepareProductSpecificationAttributeDtoAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            throw new NotImplementedException();
        }
    }
}
