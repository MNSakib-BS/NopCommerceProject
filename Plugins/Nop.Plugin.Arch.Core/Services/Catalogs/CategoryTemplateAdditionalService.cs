using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Services.Caching.Extensions;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Catalogs;
public class CategoryTemplateAdditionalService : ICategoryTemplateAdditionalService
{
    #region Fields

    private readonly IRepository<CategoryTemplateAdditional> _categoryTemplateAdditionalRepository;
    private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
    private readonly IStoreMappingService _storeMappingService;
    private readonly ICacheKeyService _cacheKeyService;

    #endregion

    #region Ctor

    public CategoryTemplateAdditionalService(IRepository<CategoryTemplateAdditional> categoryTemplateAdditionalRepository,
        IRepository<CategoryTemplate> categoryTemplateRepository,
        IStoreMappingService storeMappingService,
        ICacheKeyService cacheKeyService)
    {
        _categoryTemplateAdditionalRepository = categoryTemplateAdditionalRepository;
        _categoryTemplateRepository = categoryTemplateRepository;
        _storeMappingService = storeMappingService;
        _cacheKeyService = cacheKeyService;
    }

    #endregion

    #region Utilites

    #endregion

    #region Methods

    public virtual async Task DeleteCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional)
    {
        await _categoryTemplateAdditionalRepository.DeleteAsync(categoryAdditional);
    }

    public virtual async Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync(int storeId = 0)
    {
        var query = from pt in _categoryTemplateRepository.Table
                    select pt;

        var ctaQuery = from cta in _categoryTemplateAdditionalRepository.Table
                       join ct in query on cta.CategoryTemplateId equals ct.Id
                       select cta;

        ctaQuery = await _storeMappingService.ApplyStoreMapping(ctaQuery, storeId);

        query = from pt in query
                join cta in ctaQuery on pt.Id equals cta.CategoryTemplateId
                orderby pt.DisplayOrder, pt.Id
                select pt;

        var categoryTemplates = await query.ToCachedListAsync(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryTemplatesAllCacheKey));

        return categoryTemplates;
    }


    public virtual async Task<CategoryTemplateAdditional> GetCategoryTemplateAdditionalByCategoryTemplateIdAsync(int categoryTemplateId)
    {
        return await _categoryTemplateAdditionalRepository.Table.Where(e => e.CategoryTemplateId == categoryTemplateId).FirstOrDefaultAsync();
    }

    public virtual async Task<CategoryTemplateAdditional> GetCategoryTemplateAdditionalByIdAsync(int id)
    {
        return await _categoryTemplateAdditionalRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);
    }

    public virtual async Task InsertCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional)
    {
        await _categoryTemplateAdditionalRepository.InsertAsync(categoryAdditional);
    }

    public virtual async Task UpdateCategoryTemplateAdditionalAsync(CategoryTemplateAdditional categoryAdditional)
    {
        await _categoryTemplateAdditionalRepository.UpdateAsync(categoryAdditional);
    }

    #endregion

}
