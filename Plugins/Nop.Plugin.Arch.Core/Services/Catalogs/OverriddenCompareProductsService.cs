using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Security;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Services.Common;
using Nop.Services.Catalog;
using Nop.Services.Common;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class OverriddenCompareProductsService : CompareProductsService
{

    #region Fields

    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IGenericAttributeAdditionalService _genericAttributeAdditionalService;

    #endregion

    #region Ctor

    public OverriddenCompareProductsService(CatalogSettings catalogSettings,
        CookieSettings cookieSettings,
        IHttpContextAccessor httpContextAccessor,
        IProductService productService,
        IWebHelper webHelper,
        IGenericAttributeService genericAttributeService,
        IGenericAttributeAdditionalService genericAttributeAdditionalService) : base(catalogSettings, cookieSettings, httpContextAccessor, productService, webHelper)
    {

        _genericAttributeService = genericAttributeService;
        _genericAttributeAdditionalService = genericAttributeAdditionalService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get a list of identifier of compared products
    /// </summary>
    /// <returns>List of identifier</returns>
    protected override List<int> GetComparedProductIds()
    {
        var keyGroup = typeof(Product).Name;

        var attributes = _genericAttributeAdditionalService.GetAttributesByKey(NopCustomerDefaultsAdditional.CompareProductAttribute);

        return attributes.Select(i => i.EntityId).Distinct().ToList();

    }

    #endregion

    #region methods

    /// <summary>
    /// Clears a "compare products" list
    /// </summary>
    public override void ClearCompareProducts()
    {
        _genericAttributeAdditionalService.DeleteAttributesByKey(NopCustomerDefaultsAdditional.CompareProductAttribute);
    }

    /// <summary>
    /// Removes a product from a "compare products" list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    public override Task RemoveProductFromCompareListAsync(int productId)
    {
        _genericAttributeAdditionalService.DeleteAttributeByEntityId(NopCustomerDefaultsAdditional.CompareProductAttribute, productId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a product to a "compare products" list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    public override Task AddProductToCompareListAsync(int productId)
    {
        var product = _productService.GetProductByIdAsync(productId);

        _genericAttributeService.SaveAttributeAsync<int>(product.Result, NopCustomerDefaultsAdditional.CompareProductAttribute, productId);

        return Task.CompletedTask;
    }


    #endregion



}
