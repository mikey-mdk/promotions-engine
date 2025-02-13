using Serilog;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class RedisCacheExtensions
{
    private const string RedisConnectionStringName = "RedisInCluster";

    public static async Task AddRedisCacheAsync(this WebApplicationBuilder builder)
    {
        var loggerFactory = LoggerFactory.Create(x => x.AddSerilog());

        var configurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            EndPoints = new EndPointCollection
                { builder.Configuration.GetConnectionString(RedisConnectionStringName) ?? string.Empty },
            LoggerFactory = loggerFactory
        };

        var instance = await ConnectionMultiplexer.ConnectAsync(configurationOptions);

        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => instance);
    }
}