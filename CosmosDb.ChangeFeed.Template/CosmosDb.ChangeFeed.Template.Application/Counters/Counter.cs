using CosmosDb.ChangeFeed.Template.Application.Options;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.Persistence.Factories;
using CosmosDb.ChangeFeed.Template.Persistence.Repositories;
using Microsoft.Extensions.Options;

namespace CosmosDb.ChangeFeed.Template.Application.Counters;

public abstract class Counter : ICounter
{
    private readonly IRepository _repository;

    protected Counter(IOptions<AppSettingsOptions> appSettings, IRepositoriesFactory repositoriesFactory)
    {
        var databaseId = appSettings.Value.DatabaseId;
        var containerId = appSettings.Value.ContainerId;

        _repository = repositoriesFactory.CreateRepository(databaseId, containerId);
    }

    protected abstract ProductStatus Status { get; }

    public async ValueTask<int> GetCountAsync()
    {
        var count = await _repository.GetCountByStatusAsync(Status);

        return count;
    }
}
