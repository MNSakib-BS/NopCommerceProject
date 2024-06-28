using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class ProductAttributeAdditionalService : IProductAttributeAdditionalService
{
    #region Fields

    private readonly IRepository<ProductAttributeAdditional> _productAttributeAdditionalReporsitory;


    #endregion

    #region Ctor

    public ProductAttributeAdditionalService(IRepository<ProductAttributeAdditional> productAttributeAdditionalReporsitory)
    {
        _productAttributeAdditionalReporsitory = productAttributeAdditionalReporsitory;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeleteProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional)
    {
        await _productAttributeAdditionalReporsitory.DeleteAsync(productAttributeAdditional);
    }

    public virtual async Task<ProductAttributeAdditional> GetProductAttributeAdditionalByIdAsync(int id)
    {
       return await _productAttributeAdditionalReporsitory.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task<ProductAttributeAdditional> GetProductAttributeAdditionalByProductAttributeIdAsync(int productAttributeId)
    {
        return await _productAttributeAdditionalReporsitory.Table.Where(e => e.ProductAttributeId == productAttributeId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional)
    {
        await _productAttributeAdditionalReporsitory.InsertAsync(productAttributeAdditional);
    }

    public virtual async Task UpdateProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional)
    {
        await _productAttributeAdditionalReporsitory.UpdateAsync(productAttributeAdditional);
    }

    #endregion
}
