using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface IManufacturerTemplateAdditionalService
{
    Task<ManufacturerTemplateAdditional> GetManufacturerTemplateAdditionalByManufacturerTemplateIdAsync(int manufacturerTemplateId);

    Task<ManufacturerTemplateAdditional> GetManufacturerTemplateAdditionalByIdAsync(int id);

    Task DeleteManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional);

    Task InsertManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional);

    Task UpdateManufacturerTemplateAdditionalAsync(ManufacturerTemplateAdditional manufacturerTemplateAdditional);
}
