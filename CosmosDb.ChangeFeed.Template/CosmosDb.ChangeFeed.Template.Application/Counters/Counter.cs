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

    private int _callCount = 0;

    public async ValueTask<int> GetCountAsync2()
    {
        bool firstCall = _callCount == 0;

        // or any other logic to determine if need to initialize the counter,
        // e.g. each period of time (30 min), each 10th call etc.
        if (firstCall)
        {
            await InitializeCountInDatabase();
        }

        _callCount++;

        return await _repository.GetCountFromCounter(Status);
    }

    private async ValueTask InitializeCountInDatabase()
    {
        var count = await _repository.GetCountByStatusAsync(Status);

        await _repository.UpsertCounter(Status, count);
    }
}
