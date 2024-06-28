using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class ManufacturerTemplateAdditionalService : IManufacturerTemplateAdditionalService
{
    #region Fields

    private readonly IRepository<ManufacturerTemplateAdditional> _manufacturerTemplateAdditionalRepository;

    #endregion

    #region Ctor

    public ManufacturerTemplateAdditionalService(IRepository<ManufacturerTemplateAdditional> manufacturerTemplateAdditionalRepository)
    {
        _manufacturerTemplateAdditionalRepository = manufacturerTemplateAdditionalRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeleteManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional)
    {
        await _manufacturerTemplateAdditionalRepository.DeleteAsync(manufacturerTemplateAdditional);
    }

    public virtual async Task<ManufacturerTemplateAdditional> GetManufacturerTemplateAdditionalByIdAsync(int id)
    {
        return await _manufacturerTemplateAdditionalRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task<ManufacturerTemplateAdditional> GetManufacturerTemplateAdditionalByManufacturerTemplateIdAsync(int manufacturerTemplateId)
    {
        return await _manufacturerTemplateAdditionalRepository.Table.Where(e => e.ManufacturerTemplateId == manufacturerTemplateId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional)
    {
        await _manufacturerTemplateAdditionalRepository.InsertAsync(manufacturerTemplateAdditional);
    }

    public virtual async Task UpdateManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional)
    {
        await _manufacturerTemplateAdditionalRepository.UpdateAsync(manufacturerTemplateAdditional);
    }

    #endregion
}
