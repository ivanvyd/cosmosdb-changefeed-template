using CosmosDb.ChangeFeed.Template.Persistence.Repositories;
using Microsoft.Azure.Cosmos;

namespace CosmosDb.ChangeFeed.Template.Persistence.Factories;

public sealed class CosmosRepositoriesFactory(CosmosClient cosmosClient) : IRepositoriesFactory
{
    private readonly CosmosClient _cosmosClient = cosmosClient;

    public IRepository CreateRepository(string databaseId, string containerId)
    {
        var container = _cosmosClient.GetContainer(databaseId, containerId);

        return new CosmosRepository(container);
    }
}
