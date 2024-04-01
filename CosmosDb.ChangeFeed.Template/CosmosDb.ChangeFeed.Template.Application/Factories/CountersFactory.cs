using CosmosDb.ChangeFeed.Template.Application.Counters;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDb.ChangeFeed.Template.Application.Factories;

public sealed class CountersFactory(IServiceProvider serviceProvider) : ICountersFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ICounter CreateCounter(ProductStatus productStatus) => _serviceProvider
        .GetRequiredKeyedService<ICounter>(productStatus.ToString());
}
