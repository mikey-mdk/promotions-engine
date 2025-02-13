namespace PromotionsEngine.Application.Cache.Interfaces;

/// <summary>
/// Stubbing out this interface for now to allow its consumption.
/// This implementation of these methods will come in a later PR.
/// </summary>
public interface IRedisCacheManager
{
    /// <summary>
    /// Attempts to get the value from the cache, if it does not exist, it will call the action to get the value and store it in the cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> action);

    /// <summary>
    /// Invalidates the current cache of the provided cache key.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    Task InvalidateCache(string cacheKey);
}