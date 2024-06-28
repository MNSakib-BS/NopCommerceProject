using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class ProductAttributeMappingAdditionalService : IProductAttributeMappingAdditionalService
{
    #region Fields

    private readonly IRepository<ProductAttributeMappingAdditional> _productAttributeMappingRepository;


    #endregion

    #region Ctor

    public ProductAttributeMappingAdditionalService(IRepository<ProductAttributeMappingAdditional> productAttributeMappingRepository)
    {
        _productAttributeMappingRepository = productAttributeMappingRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods
    public virtual async Task DeleteProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional)
    {
        await _productAttributeMappingRepository.DeleteAsync(productAttributeMappingAdditional);
    }

    public virtual async Task<ProductAttributeMappingAdditional> GetProductAttributeMappingAdditionalByIdAsync(int id)
    {
        return await _productAttributeMappingRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task<ProductAttributeMappingAdditional> GetProductAttributeMappingAdditionalByPAMIdAsync(int productAttributeMappingId)
    {
        return await _productAttributeMappingRepository.Table.Where(e => e.ProductAttributeMappingId == productAttributeMappingId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional)
    {
        await _productAttributeMappingRepository.InsertAsync(productAttributeMappingAdditional);
    }

    public virtual async Task UpdateProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional)
    {
        await _productAttributeMappingRepository.UpdateAsync(productAttributeMappingAdditional);
    }

    #endregion

}
