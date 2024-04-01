using CosmosDb.ChangeFeed.Template.Domain.Entities;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using Microsoft.Azure.Cosmos;

namespace CosmosDb.ChangeFeed.Template.Application.Infrastructure;


/// <summary>
/// Initializes the Cosmos DB database and container.
/// </summary>
public sealed class CosmosDbInitializer
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbInitializer"/> class.
    /// </summary>
    /// <param name="connectionString">The Cosmos DB connection string.</param>
    public CosmosDbInitializer(string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        _connectionString = connectionString;
    }

    private string? _databaseId;
    private string? _containerId;
    private bool _seedData = false;
    private uint _itemsCount = 10;

    /// <summary>
    /// Sets the database ID.
    /// </summary>
    /// <param name="databaseId">The database ID.</param>
    /// <returns>The <see cref="CosmosDbInitializer"/> instance.</returns>
    public CosmosDbInitializer CreateDatabase(string databaseId)
    {
        _databaseId = databaseId;
        return this;
    }

    /// <summary>
    /// Sets the container ID.
    /// </summary>
    /// <param name="containerId">The container ID.</param>
    /// <returns>The <see cref="CosmosDbInitializer"/> instance.</returns>
    public CosmosDbInitializer CreateContainer(string containerId)
    {
        _containerId = containerId;
        return this;
    }

    /// <summary>
    /// Enables seeding of data.
    /// </summary>
    /// <param name="itemsCount">The number of items to seed. Default is 10.</param>
    /// <returns>The <see cref="CosmosDbInitializer"/> instance.</returns>
    public CosmosDbInitializer WithSeededData(uint itemsCount = 10)
    {
        _seedData = true;
        _itemsCount = itemsCount;
        return this;
    }

    /// <summary>
    /// Initializes the Cosmos DB database and container.
    /// </summary>
    /// <returns>The initialized <see cref="Container"/>.</returns>
    public async Task<Container> Initialize()
    {
        using var cosmosClientWithBulk = new CosmosClient(_connectionString, new CosmosClientOptions
        {
            AllowBulkExecution = true,
        });

        var database = await cosmosClientWithBulk.CreateDatabaseIfNotExistsAsync(_databaseId);

        var containerProperties = new ContainerProperties(_containerId, "/id");

        var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerProperties);

        var container = containerResponse.Container;

        if (_seedData)
        {
            await SeedData(container);
        }

        return container;
    }

    private async Task SeedData(Container container)
    {
        var random = new Random();

        var tasks = new List<Task>();

        for (var i = 0; i < _itemsCount; i++)
        {
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = GenerateRandomName(random),
                Status = GenerateRandomStatus(random),
            };

            var task = container.CreateItemAsync(product, new PartitionKey(product.Id));

            tasks.Add(task.ContinueWith(t =>
            {
                if (t.Status is TaskStatus.Faulted)
                {
                    Console.WriteLine($"Error while creating item: {t.Exception?.Message}");
                }
                else
                {
                    Console.WriteLine($"Item created: {product}");
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    private static string GenerateRandomName(Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var randomCharsArray = Enumerable.Repeat(chars, 10)
            .Select(s => s[random.Next(s.Length)])
            .ToArray();

        var randomString = new string(randomCharsArray);

        return $"Product {randomString}";
    }

    private static ProductStatus GenerateRandomStatus(Random random)
    {
        var values = Enum.GetValues<ProductStatus>();

        return (ProductStatus)values!.GetValue(random.Next(values.Length))!;
    }
}
