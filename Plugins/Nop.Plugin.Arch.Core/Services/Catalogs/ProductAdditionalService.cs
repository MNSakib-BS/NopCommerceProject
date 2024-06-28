using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class ProductAdditionalService : IProductAdditionalService
{
    #region Fields

    private readonly IRepository<ProductAdditional> _productAdditionalRepositoty;

    #endregion

    #region Ctor

    public ProductAdditionalService(IRepository<ProductAdditional> productAdditionalRepositoty)
    {
        _productAdditionalRepositoty = productAdditionalRepositoty;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeleteProductAdditionalAsync(ProductAdditional productAdditional)
    {
        await _productAdditionalRepositoty.DeleteAsync(productAdditional);
    }

    public virtual async Task<ProductAdditional> GetProductAdditionalByIdAsync(int id)
    {
       return await _productAdditionalRepositoty.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task<ProductAdditional> GetProductAdditionalByProductIdAsync(int productId)
    {
        return await _productAdditionalRepositoty.Table.Where(e => e.ProductId == productId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertProductAdditionalAsync(ProductAdditional productAdditional)
    {
        await _productAdditionalRepositoty.InsertAsync(productAdditional);
    }

    public virtual async Task UpdateProductAdditionalAsync(ProductAdditional productAdditional)
    {
       await _productAdditionalRepositoty.UpdateAsync(productAdditional);
    }

    #endregion
}
