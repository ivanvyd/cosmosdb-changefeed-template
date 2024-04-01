using CosmosDb.ChangeFeed.Template.Application.Counters;
using CosmosDb.ChangeFeed.Template.Domain.Enums;

namespace CosmosDb.ChangeFeed.Template.Application.Factories;

public interface ICountersFactory
{
    ICounter CreateCounter(ProductStatus productStatus);
}
