using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class PredefinedProductAttributeValueAdditionalService : IPredefinedProductAttributeValueAdditionalService
{
    #region Fields

    private readonly IRepository<PredefinedProductAttributeValueAdditional> _predefinedProductAttributeValueAdditionalRepository;


    #endregion

    #region Ctor

    public PredefinedProductAttributeValueAdditionalService(IRepository<PredefinedProductAttributeValueAdditional> predefinedProductAttributeValueAdditionalRepository)
    {
        _predefinedProductAttributeValueAdditionalRepository = predefinedProductAttributeValueAdditionalRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeletePredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional)
    {
        await _predefinedProductAttributeValueAdditionalRepository.DeleteAsync(predefinedProductAttributeValueAdditional);
    }

    public virtual async Task<PredefinedProductAttributeValueAdditional> GetPredefinedProductAttributeValueAdditionalByIdAsync(int id)
    {
        return await _predefinedProductAttributeValueAdditionalRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task<PredefinedProductAttributeValueAdditional> GetPredefinedProductAttributeValueAdditionalByPredefinedPAVIdAsync(int predefinedProductAttributeValueId)
    {
        return await _predefinedProductAttributeValueAdditionalRepository.Table.Where(e => e.PredefinedProductAttributeValueId == predefinedProductAttributeValueId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertPredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional)
    {
        await _predefinedProductAttributeValueAdditionalRepository.InsertAsync(predefinedProductAttributeValueAdditional);
    }

    public virtual async Task UpdatePredefinedProductAttributeValueAdditionalAsync(PredefinedProductAttributeValueAdditional predefinedProductAttributeValueAdditional)
    {
        await _predefinedProductAttributeValueAdditionalRepository.UpdateAsync(predefinedProductAttributeValueAdditional);
    }


    #endregion


}
