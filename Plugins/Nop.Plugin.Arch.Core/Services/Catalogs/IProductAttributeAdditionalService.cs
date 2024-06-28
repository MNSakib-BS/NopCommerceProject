using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IProductAttributeAdditionalService
{
    Task<ProductAttributeAdditional> GetProductAttributeAdditionalByProductAttributeIdAsync(int productAttributeId);

    Task<ProductAttributeAdditional> GetProductAttributeAdditionalByIdAsync(int id);

    Task DeleteProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional);

    Task InsertProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional);

    Task UpdateProductAttributeAdditionalAsync(ProductAttributeAdditional productAttributeAdditional);
}
