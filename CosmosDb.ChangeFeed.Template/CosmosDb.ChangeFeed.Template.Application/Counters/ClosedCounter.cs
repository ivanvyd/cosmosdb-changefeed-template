using CosmosDb.ChangeFeed.Template.Application.Options;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.Persistence.Factories;
using Microsoft.Extensions.Options;

namespace CosmosDb.ChangeFeed.Template.Application.Counters;

public sealed class ClosedCounter(IOptions<AppSettingsOptions> appSettings, IRepositoriesFactory repositoriesFactory) : Counter(appSettings, repositoriesFactory)
{
    protected override ProductStatus Status => ProductStatus.Closed;
}
