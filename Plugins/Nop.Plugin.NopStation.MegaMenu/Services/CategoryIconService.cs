using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.MegaMenu.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public class CategoryIconService : ICategoryIconService
    {
        private readonly IRepository<CategoryIcon> _categoryIconRepository;

        public CategoryIconService(IRepository<CategoryIcon> categoryIconRepository)
        {
            _categoryIconRepository = categoryIconRepository;
        }

        public async Task DeleteCategoryIconAsync(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            await _categoryIconRepository.DeleteAsync(categoryIcon);
        }

        public async Task InsertCategoryIconAsync(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            await _categoryIconRepository.InsertAsync(categoryIcon);
        }

        public async Task UpdateCategoryIconAsync(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            await _categoryIconRepository.UpdateAsync(categoryIcon);
        }

        public async Task<CategoryIcon> GetCategoryIconByIdAsync(int categoryIconId)
        {
            if (categoryIconId == 0)
                return null;

            return await _categoryIconRepository.GetByIdAsync(categoryIconId);
        }

        public async Task<CategoryIcon> GetCategoryIconByCategoryIdAsync(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return await _categoryIconRepository.Table.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        }

        public async Task<IPagedList<CategoryIcon>> GetAllCategoryIconsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var categoryIcons = _categoryIconRepository.Table.OrderByDescending(e => e.Id);
            return await categoryIcons.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<IList<CategoryIcon>> GetCategoryIconByIdsAsync(int[] categoryIconIds)
        {
            if (categoryIconIds == null || categoryIconIds.Length == 0)
                return new List<CategoryIcon>();

            var query = _categoryIconRepository.Table.Where(x => categoryIconIds.Contains(x.Id));
            return await query.ToListAsync();
        }

        public async Task DeleteCategoryIconsAsync(List<CategoryIcon> categoryIcons)
        {
            if (categoryIcons == null)
                throw new ArgumentNullException(nameof(categoryIcons));

            foreach (var categoryIcon in categoryIcons)
            {
                await _categoryIconRepository.DeleteAsync(categoryIcon);
            }
        }
    }
}
