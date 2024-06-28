using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Arch.Core.Domains.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public interface ICategoryTemplateAdditionalService
{   
    Task<CategoryTemplateAdditional> GetCategoryTemplateAdditionalByCategoryTemplateIdAsync(int categoryTemplateId);
    
    Task<CategoryTemplateAdditional> GetCategoryTemplateAdditionalByIdAsync(int id);
    
    Task DeleteCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional);

    Task InsertCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional);
    
    Task UpdateCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional);
    //custom
    Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync(int storeId = 0);
}
