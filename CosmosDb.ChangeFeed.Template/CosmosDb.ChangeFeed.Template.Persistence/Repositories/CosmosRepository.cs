using CosmosDb.ChangeFeed.Template.Domain.Entities;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CosmosDb.ChangeFeed.Template.Persistence.Repositories;

public sealed class CosmosRepository : IRepository
{
    private readonly Container _container;
    private readonly ILogger<CosmosRepository> _logger;

    public CosmosRepository(Container container, ILogger<CosmosRepository> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async Task CreateAsync(Product product)
    {
        await _container.CreateItemAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        await _container.ReplaceItemAsync(product, product.Id);
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Product>(id, PartitionKey.None);
    }

    public async Task<Product?> GetAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Product>(id, PartitionKey.None);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c");

        using var iterator = _container.GetItemQueryIterator<Product>(query);

        var results = new List<Product>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            results.AddRange([.. response]);
        }

        return results;
    }

    public async Task<List<string>> GetByStatusAsync(ProductStatus status)
    {
        var query = new QueryDefinition($"SELECT c.id FROM c WHERE c.{nameof(Product.Status)} = @status")
            .WithParameter("@status", status);

        using var iterator = _container.GetItemQueryIterator<string>(query);

        var results = new List<string>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            results.AddRange([.. response]);
        }

        return results;
    }

    public async Task<int> GetCountByStatusAsync(ProductStatus status)
    {
        var query = new QueryDefinition($"SELECT VALUE COUNT(1) FROM c WHERE c.{nameof(Product.Status)} = @status")
            .WithParameter("@status", status);

        using var iterator = _container.GetItemQueryIterator<int>(query);
        var response = await iterator.ReadNextAsync();

        _logger.LogInformation("{queryName} query for {status} status consumed {responseRequestCharge} RUs.",
            "GetCountByStatusAsync",
            status,
            response.RequestCharge);

        return response.FirstOrDefault();
    }

    public async Task UpsertCounter(ProductStatus productStatus, int count)
    {
        var counter = new Counter
        {
            Id = productStatus.ToString(),
            Count = count,
            Type = typeof(Counter).AssemblyQualifiedName,
        };

        var response = await _container.UpsertItemAsync(counter, new PartitionKey(productStatus.ToString()));

        _logger.LogInformation("{queryName} for {status} status consumed {responseRequestCharge} RUs.",
            "UpsertItemAsync",
            productStatus,
            response.RequestCharge);
    }

    public async Task<int> GetCountFromCounter(ProductStatus status)
    {
        var counterItemResponse = await _container.ReadItemAsync<Counter>(status.ToString(), new PartitionKey(status.ToString()));

        _logger.LogInformation("{queryName} for {status} status consumed {responseRequestCharge} RUs.",
            "GetCountFromCounter",
            status,
            counterItemResponse.RequestCharge);

        if (counterItemResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return 0;
        }

        return counterItemResponse.Resource.Count;
    }
}
