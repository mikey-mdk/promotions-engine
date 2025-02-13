using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PromotionsEngine.Application;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Configuration;
using PromotionsEngine.Domain;
using PromotionsEngine.Infrastructure;
using PromotionsEngine.Infrastructure.ChangeFeed.Interfaces;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.WebApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Host
    .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var logger = LoggerFactory.Create(x => x.AddSerilog()).CreateLogger<Program>();
logger.LogInformation("Starting PromotionsEngine WebApi");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationStartup()
                .AddDomainStartup()
                .AddInfrastructureStartup();

// Options Configuration
builder.Services.Configure<CosmosDbOptions>(options => builder.Configuration.GetSection(CosmosDbOptions.CosmosDbOptionsSectionName).Bind(options));

builder.Services.Configure<ServiceBusOptions>(options => builder.Configuration.GetSection(ServiceBusOptions.ServiceBusSectionName).Bind(options));

await builder.AddAzureClientConnections();
await builder.AddRedisCacheAsync();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

await app.Services.GetRequiredService<ICosmosChangeFeedProcessor>().SetupCosmosChangeFeedProcessors();
await app.Services.GetRequiredService<IMerchantRegexLookupCacheManager>().HydrateMerchantRegexLookupCache();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UsePathBase("/promotions-engine").UseRouting();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "PromotionsEngine");
    });
}

app.Run();

public partial class Program { }