using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public interface IShoppingCartItemApiService
    {
        Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync(
            int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Constants.Configurations.DefaultLimit,
            int page = Constants.Configurations.DefaultPageValue);

        Task<ShoppingCartItem> GetShoppingCartItemAsync(int id);
    }
}
