﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Arch.Core.Services.Catalogs.Extensions;
public static class ArchProductExtensions
{
    /// <summary>
    /// Sorts the elements of a sequence in order according to a product sorting rule
    /// </summary>
    /// <param name="productsQuery">A sequence of products to order</param>
    /// <param name="currentLanguage">Current language</param>
    /// <param name="orderBy">Product sorting rule</param>
    /// <param name="localizedPropertyRepository">Localized property repository</param>
    /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
    /// <remarks>
    /// If <paramref name="orderBy"/> is set to <c>Position</c> and passed <paramref name="productsQuery"/> is
    /// ordered sorting rule will be skipped
    /// </remarks>
    public static async Task<IQueryable<Product>> OrderByAsync(this IQueryable<Product> productsQuery, ProductSortingEnum orderBy)
    {
        if (orderBy == ProductSortingEnum.NameAsc || orderBy == ProductSortingEnum.NameDesc)
        {
            var query =
                from product in productsQuery
                select product;

            if (orderBy == ProductSortingEnum.NameAsc)
                productsQuery = from item in query
                                orderby item.Name
                                select item;
            else
                productsQuery = from item in query
                                orderby item.Name descending
                                select item;

            return await Task.FromResult(productsQuery);
        }

        return await Task.FromResult(orderBy switch
        {
            ProductSortingEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
            ProductSortingEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
            ProductSortingEnum.CreatedOn => productsQuery.OrderByDescending(p => p.CreatedOnUtc),
            ProductSortingEnum.Position when productsQuery is IOrderedQueryable => productsQuery,
            _ => productsQuery.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id)
        });
    }
}

