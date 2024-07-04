using System;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Factories
{
    public class ShoppingCartItemFactory : IFactory<ShoppingCartItem>
    {
        public async Task<ShoppingCartItem> InitializeAsync()
        {
            var newShoppingCartItem = new ShoppingCartItem();

            newShoppingCartItem.CreatedOnUtc = DateTime.UtcNow;
            newShoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            return newShoppingCartItem;
        }
    }
}
