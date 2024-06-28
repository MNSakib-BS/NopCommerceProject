using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IProductAttributeMappingAdditionalService
{
    /// <summary>
    /// PAM-- ProductAttributeMapping
    /// </summary>
    /// <param name="productAttributeMappingId"></param>
    /// <returns></returns>
    Task<ProductAttributeMappingAdditional> GetProductAttributeMappingAdditionalByPAMIdAsync(int productAttributeMappingId);

    Task<ProductAttributeMappingAdditional> GetProductAttributeMappingAdditionalByIdAsync(int id);

    Task DeleteProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional);

    Task InsertProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional);

    Task UpdateProductAttributeMappingAdditionalAsync(ProductAttributeMappingAdditional productAttributeMappingAdditional);
}
