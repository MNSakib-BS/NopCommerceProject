using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IProductAdditionalService
{
    Task<ProductAdditional> GetProductAdditionalByProductIdAsync(int productId);

    Task<ProductAdditional> GetProductAdditionalByIdAsync(int id);

    Task DeleteProductAdditionalAsync(ProductAdditional productAdditional);

    Task InsertProductAdditionalAsync(ProductAdditional productAdditional);

    Task UpdateProductAdditionalAsync(ProductAdditional productAdditional);
}
