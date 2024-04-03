using CosmosDb.ChangeFeed.Template.Persistence.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CosmosDb.ChangeFeed.Template.Persistence.Factories;

public sealed class CosmosRepositoriesFactory : IRepositoriesFactory
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CosmosRepository> _logger;

    public CosmosRepositoriesFactory(CosmosClient cosmosClient, ILogger<CosmosRepository> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    public IRepository CreateRepository(string databaseId, string containerId)
    {
        var container = _cosmosClient.GetContainer(databaseId, containerId);

        return new CosmosRepository(container, _logger);
    }
}
