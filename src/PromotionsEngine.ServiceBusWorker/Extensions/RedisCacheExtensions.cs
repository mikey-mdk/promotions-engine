using Microsoft.AspNetCore.Builder;
using StackExchange.Redis;
using Serilog;

namespace PromotionsEngine.ServiceBusWorker.Extensions;

public static class RedisCacheExtensions
{
    private const string RedisConnectionStringName = "RedisInCluster";

    public static async Task AddRedisCacheAsync(this HostApplicationBuilder builder)
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