using Serilog;
using Serilog.Core;
using PromotionsEngine.Application;
using PromotionsEngine.Application.BusMessageHandlers.Implementations;
using PromotionsEngine.Application.BusMessageHandlers.Interfaces;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.CommandHandlers.Implementations;
using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Domain;
using PromotionsEngine.Infrastructure;
using PromotionsEngine.ServiceBusWorker;
using PromotionsEngine.ServiceBusWorker.Configuration;
using PromotionsEngine.ServiceBusWorker.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ServiceBusWorker>();

builder.Services.AddLogging(loggingBuilder =>
{
    Logger logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

    loggingBuilder.AddSerilog(logger, dispose: true);
});

var logger = LoggerFactory.Create(x => x.AddSerilog()).CreateLogger<Program>();
logger.LogInformation("Starting PromotionsEngine ServiceBusWorker");

builder.Services.Configure<ServiceBusOptions>(options =>
    builder.Configuration.GetSection(ServiceBusOptions.ServiceBusSectionName).Bind(options));

builder.Services.AddTransient<IPromotionsEngineTransactionMessageHandler, PromotionsEngineTransactionMessageHandler>();
builder.Services.AddTransient<IOrderCreatedCommandHandler, OrderCreatedCommandHandler>();
builder.Services.AddTransient<IOrderRefundedCommandHandler, OrderRefundedCommandHandler>();
builder.Services.AddTransient<IOrderSettledCommandHandler, OrderSettledCommandHandler>();

builder.Services.AddApplicationStartup()
                .AddDomainStartup()
                .AddInfrastructureStartup();

await builder.AddAzureClientConnections();
await builder.AddRedisCacheAsync();

var host = builder.Build();

await host.Services.GetRequiredService<IMerchantRegexLookupCacheManager>().HydrateMerchantRegexLookupCache();

host.Run();
