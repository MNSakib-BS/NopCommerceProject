﻿using Nop.Core;
using Nop.Plugin.NopStation.MegaMenu.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public interface ICategoryIconService
    {
        Task DeleteCategoryIconAsync(CategoryIcon categoryIcon);

        Task InsertCategoryIconAsync(CategoryIcon categoryIcon);

        Task UpdateCategoryIconAsync(CategoryIcon categoryIcon);

        Task<CategoryIcon> GetCategoryIconByIdAsync(int categoryIconId);

        Task<IList<CategoryIcon>> GetCategoryIconByIdsAsync(int[] categoryIconIds);

        Task<CategoryIcon> GetCategoryIconByCategoryIdAsync(int categoryId);

        Task<IPagedList<CategoryIcon>> GetAllCategoryIconsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task DeleteCategoryIconsAsync(List<CategoryIcon> categoryIcons);
    }
}
