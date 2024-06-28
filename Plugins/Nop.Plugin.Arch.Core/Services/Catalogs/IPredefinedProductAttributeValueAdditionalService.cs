using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IPredefinedProductAttributeValueAdditionalService
{
    Task<PredefinedProductAttributeValueAdditional> GetPredefinedProductAttributeValueAdditionalByPredefinedPAVIdAsync(int predefinedProductAttributeValueId);

    Task<PredefinedProductAttributeValueAdditional> GetPredefinedProductAttributeValueAdditionalByIdAsync(int id);

    Task DeletePredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional);

    Task InsertPredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional);

    Task UpdatePredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional);
}
