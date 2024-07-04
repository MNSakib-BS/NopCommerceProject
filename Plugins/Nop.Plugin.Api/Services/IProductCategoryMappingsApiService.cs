﻿using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public interface IProductCategoryMappingsApiService
    {
        Task<IList<ProductCategory>> GetMappingsAsync(
            int? productId = null, int? categoryId = null, int limit = Constants.Configurations.DefaultLimit,
            int page = Constants.Configurations.DefaultPageValue, int sinceId = Constants.Configurations.DefaultSinceId);

        Task<int> GetMappingsCountAsync(int? productId = null, int? categoryId = null);

        Task<ProductCategory> GetByIdAsync(int id);
    }
}
