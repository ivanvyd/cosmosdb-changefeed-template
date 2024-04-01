using CosmosDb.ChangeFeed.Template.Persistence.Repositories;

namespace CosmosDb.ChangeFeed.Template.Persistence.Factories;

public interface IRepositoriesFactory
{
    IRepository CreateRepository(string databaseId, string containerId);
}
