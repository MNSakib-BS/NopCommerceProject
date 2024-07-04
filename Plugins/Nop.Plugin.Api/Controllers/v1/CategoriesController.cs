﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Categories;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    public class CategoriesController : SecureBaseApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IFactory<Category> _factory;
        private readonly IUrlRecordService _urlRecordService;

        public CategoriesController(
            ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IFactory<Category> factory,
            IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                                         customerActivityService, localizationService, pictureService)
        {
            _categoryApiService = categoryApiService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _factory = factory;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task <IActionResult> GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            var allCategories =(await _categoryApiService.GetCategoriesAsync(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                  parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                  parameters.Limit, parameters.Page, parameters.SinceId,
                                                                  parameters.ProductId, parameters.PublishedStatus))
                                                   .WhereAwait(async c =>await StoreMappingService.AuthorizeAsync(c));

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(async category =>await _dtoHelper.PrepareCategoryDTOAsync(category)).ToList();

            var categoriesRootObject = new CategoriesRootObject
                                       {
                                           Categories = categoriesAsDtos
                                       };

            var json = JsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/count")]
        [ProducesResponseType(typeof(CategoriesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            var allCategoriesCount =await _categoryApiService.GetCategoriesCountAsync(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject
                                            {
                                                Count = allCategoriesCount
                                            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        ///     Retrieve category by specified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetCategoryById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var category =await _categoryApiService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            var categoryDto =await _dtoHelper.PrepareCategoryDTOAsync(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            var json = JsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateCategory(
            [ModelBinder(typeof(JsonModelBinder<CategoryDto>))]
            Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.

            Picture insertedPicture = null;

            Picture insertedCategoryPicture = null;

            Picture insertedCategoryPictureLower = null;

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.Image?.Binary != null)
            {
                insertedPicture =await PictureService.InsertPictureAsync(categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType, string.Empty);
            }

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.BannerHeroPictureTop?.Binary != null)
            {
                insertedCategoryPicture =await PictureService.InsertPictureAsync(categoryDelta.Dto.BannerHeroPictureTop.Binary, categoryDelta.Dto.BannerHeroPictureTop.MimeType, string.Empty);
            }

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.BannerHeroPictureLower?.Binary != null)
            {
                insertedCategoryPictureLower = await PictureService.InsertPictureAsync(categoryDelta.Dto.BannerHeroPictureLower.Binary, categoryDelta.Dto.BannerHeroPictureLower.MimeType, string.Empty);
            }

            // Inserting the new category
            var category = await _factory.InitializeAsync();
            categoryDelta.Merge(category);

            if (insertedPicture != null)
            {
                category.PictureId = insertedPicture.Id;
            }

            if (insertedCategoryPicture != null)
            {
                category.BannerHeroPictureTopId = insertedCategoryPicture.Id;
            }

            if (insertedCategoryPictureLower != null)
            {
                category.BannerHeroPictureLowerId = insertedCategoryPictureLower.Id;
            }

            await _categoryService.InsertCategoryAsync(category);


            UpdateAclRoles(category, categoryDelta.Dto.RoleIds);

            UpdateDiscounts(category, categoryDelta.Dto.DiscountIds);

            UpdateStoreMappings(category, categoryDelta.Dto.StoreIds);

            //search engine name
            if (categoryDelta.Dto.SeName != null)
            {
                var seName =await _urlRecordService.ValidateSeNameAsync(category, categoryDelta.Dto.SeName, category.Name, true);
                _urlRecordService.SaveSlugAsync(category, seName, 0);
            }

            await CustomerActivityService.InsertActivityAsync("AddNewCategory",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category);

            // Preparing the result dto of the new category
            var newCategoryDto =await _dtoHelper.PrepareCategoryDTOAsync(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = JsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCategory(
            [ModelBinder(typeof(JsonModelBinder<CategoryDto>))]
            Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var category =await _categoryApiService.GetCategoryByIdAsync(categoryDelta.Dto.Id);

            if (category == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            categoryDelta.Merge(category);

            category.UpdatedOnUtc = DateTime.UtcNow;

            await _categoryService.UpdateCategoryAsync(category);

            UpdatePicture(category, categoryDelta.Dto.Image);

            UpdatePicture(category, categoryDelta.Dto.BannerHeroPictureTop);

            UpdatePicture(category, categoryDelta.Dto.BannerHeroPictureLower);

            UpdateAclRoles(category, categoryDelta.Dto.RoleIds);

            UpdateDiscounts(category, categoryDelta.Dto.DiscountIds);

            UpdateStoreMappings(category, categoryDelta.Dto.StoreIds);

            //search engine name
            if (categoryDelta.Dto.SeName != null)
            {
                var seName = await _urlRecordService.ValidateSeNameAsync(category, categoryDelta.Dto.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, seName, 0);
            }

            await _categoryService.UpdateCategoryAsync(category);

            await CustomerActivityService.InsertActivityAsync("UpdateCategory",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.UpdateCategory"), category);

            var categoryDto = await _dtoHelper.PrepareCategoryDTOAsync(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            var json = JsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var categoryToDelete = await _categoryApiService.GetCategoryByIdAsync(id);

            if (categoryToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            await _categoryService.DeleteCategoryAsync(categoryToDelete);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteCategory", await LocalizationService.GetResourceAsync("ActivityLog.DeleteCategory"), categoryToDelete);

            return new RawJsonActionResult("{}");
        }

        private async Task UpdatePicture(Category categoryEntityToUpdate, ImageDto imageDto)
        {
            // no image specified then do nothing
            if (imageDto == null)
            {
                 return;
            }

            Picture updatedPicture;
            var currentCategoryPicture = await PictureService.GetPictureByIdAsync(categoryEntityToUpdate.PictureId);

            // when there is a picture set for the category
            if (currentCategoryPicture != null)
            {
                await PictureService.DeletePictureAsync(currentCategoryPicture);

                // When the image attachment is null or empty.
                if (imageDto.Binary == null)
                {
                    categoryEntityToUpdate.PictureId = 0;
                }
                else
                {
                    updatedPicture = await PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
            // when there isn't a picture set for the category
            else
            {
                if (imageDto.Binary != null)
                {
                    updatedPicture = await PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
        }

        private async Task UpdateDiscounts(Category category, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
            {
                return;
            }

            var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
            var appliedCategoryDiscount = await DiscountService.GetAppliedDiscountsAsync(category);
            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (appliedCategoryDiscount.Count(d => d.Id == discount.Id) == 0)
                    {
                        appliedCategoryDiscount.Add(discount);
                    }
                }
                else
                {
                    //remove discount
                    if (appliedCategoryDiscount.Count(d => d.Id == discount.Id) > 0)
                    {
                        appliedCategoryDiscount.Remove(discount);
                    }
                }
            }
            await _categoryService.UpdateCategoryAsync(category);
        }
    }
}