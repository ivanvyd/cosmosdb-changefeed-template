using CosmosDb.ChangeFeed.Template.Application.Counters;
using CosmosDb.ChangeFeed.Template.Application.Factories;
using CosmosDb.ChangeFeed.Template.Application.Infrastructure;
using CosmosDb.ChangeFeed.Template.Application.Options;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.Persistence.Factories;
using CosmosDb.ChangeFeed.Template.WebApi.Extensions;
using CosmosDb.ChangeFeed.Template.WebApi.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

IWebHostEnvironment environment = builder.Environment;

ConfigurationManager configuration = builder.Configuration;

configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (environment.IsLocal())
{
    configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettingsOptions>(builder.Configuration.GetSection(AppSettingsOptions.SectionName));

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

var connectionString = configuration.GetConnectionString("CosmosDb");
builder.Services.AddSingleton(provider =>
{
    var cosmosClient = new CosmosClient(connectionString);
    provider.GetService<IHostApplicationLifetime>()!.ApplicationStopping.Register(() => cosmosClient.Dispose());
    return cosmosClient;
});
builder.Services.AddSingleton<IRepositoriesFactory, CosmosRepositoriesFactory>();

builder.Services.AddKeyedSingleton<ICounter, NewCounter>(nameof(ProductStatus.New));
builder.Services.AddKeyedSingleton<ICounter, OpenCounter>(nameof(ProductStatus.Open));
builder.Services.AddKeyedSingleton<ICounter, ClosedCounter>(nameof(ProductStatus.Closed));
builder.Services.AddSingleton<ICountersFactory, CountersFactory>();

builder.Services.AddScoped<IProductsService, ProductsService>();

WebApplication app = builder.Build();

// Initialize Cosmos DB
var appSettingsOptions = app.Services.GetRequiredService<IOptions<AppSettingsOptions>>();
await new CosmosDbInitializer(connectionString)
    .CreateDatabase(appSettingsOptions.Value.DatabaseId)
    .CreateContainer(appSettingsOptions.Value.ContainerId)
    .WithSeededData()
    .Initialize();

if (app.Environment.IsLocal() || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
