using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Arch.Core.Services.Caching.Extensions;
public static class IQueryableExtensions
{
    private static IStaticCacheManager CacheManager => EngineContext.Current.Resolve<IStaticCacheManager>();

    /// <summary>
    /// Gets a cached list asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached list</returns>
    public static async Task<IList<T>> ToCachedListAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.ToListAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets a cached array asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached array</returns>
    public static async Task<T[]> ToCachedArrayAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.ToArrayAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.ToArrayAsync());
    }

    /// <summary>
    /// Gets a cached first element of a sequence, or a default value asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached first element or default value</returns>
    public static async Task<T> ToCachedFirstOrDefaultAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.FirstOrDefaultAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.FirstOrDefaultAsync());
    }

    /// <summary>
    /// Gets the only element of a sequence, and throws an exception
    /// if there is not exactly one element in the sequence asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached single element</returns>
    public static async Task<T> ToCachedSingleAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.SingleAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.SingleAsync());
    }

    /// <summary>
    /// Gets a cached value which determines whether a sequence contains any elements asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached value which determines whether a sequence contains any elements</returns>
    public static async Task<bool> ToCachedAnyAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.AnyAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.AnyAsync());
    }

    /// <summary>
    /// Gets a cached number of elements in a sequence asynchronously
    /// </summary>
    /// <typeparam name="T">The type of the elements of source</typeparam>
    /// <param name="query">Elements of source to put on cache</param>
    /// <param name="cacheKey">Cache key</param>
    /// <returns>Cached number of elements</returns>
    public static async Task<int> ToCachedCountAsync<T>(this IQueryable<T> query, CacheKey cacheKey)
    {
        if (cacheKey == null)
            return await query.CountAsync();

        return await CacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
    }
}

