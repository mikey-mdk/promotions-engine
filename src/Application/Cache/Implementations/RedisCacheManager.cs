using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using PromotionsEngine.Application.Cache.Interfaces;

namespace PromotionsEngine.Application.Cache.Implementations;

public class RedisCacheManager : IRedisCacheManager
{
    private readonly ILogger<RedisCacheManager> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    private static readonly TimeSpan Ttl = TimeSpan.FromDays(1);
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true
    };

    public RedisCacheManager(
        ILogger<RedisCacheManager> logger,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;

        if (!_connectionMultiplexer.IsConnected)
        {
            _logger.LogError("Redis Connection Failed On Startup");
        }

        _connectionMultiplexer.ConnectionFailed += (sender, args) =>
        {
            _logger.LogWarning("Redis Connection Failed");
        };

        _connectionMultiplexer.ConnectionRestored += (sender, args) =>
        {
            _logger.LogWarning("Redis Connection Restored");
        };
    }

    public async Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> action)
    {
        try
        {
            if (_connectionMultiplexer is { IsConnected: true })
            {
                var jsonResponse = await _connectionMultiplexer.GetDatabase().StringGetAsync(cacheKey);

                if (!jsonResponse.IsNullOrEmpty)
                {
                    return JsonSerializer.Deserialize<T>(jsonResponse!, _jsonSerializerOptions);
                }

                var data = await action();

                //if data returned from func is null or default there is nothing to cache so return.
                if (EqualityComparer<T>.Default.Equals(data, default))
                {
                    return data;
                }

                var cacheValue = JsonSerializer.Serialize(data, _jsonSerializerOptions);

                if (!await _connectionMultiplexer.GetDatabase().StringSetAsync(new RedisKey(cacheKey), new RedisValue(cacheValue), Ttl))
                {
                    _logger.LogWarning("Failed to write cache entry for: {key}", cacheKey);
                }

                return data;
            }

            _logger.LogError("Redis Connection Failed, falling back to action");
            return await action();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to GetOrSet cache for key: {key}", cacheKey);
            return await action();
        }
    }

    public async Task InvalidateCache(string cacheKey)
    {
        try
        {
            if (_connectionMultiplexer is { IsConnected: true }
                && !await _connectionMultiplexer.GetDatabase().KeyDeleteAsync(cacheKey))
            {
                _logger.LogWarning("Failed to delete cache entry for: {key}", cacheKey);
            }

            _logger.LogError("Redis Connection Failed, unable to invalidate cache for key: {cacheKey}", cacheKey);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to GetOrSet cache for key: {key}", cacheKey);
        }
    }
}