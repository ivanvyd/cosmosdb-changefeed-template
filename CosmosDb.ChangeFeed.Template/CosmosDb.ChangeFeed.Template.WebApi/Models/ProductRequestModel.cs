using CosmosDb.ChangeFeed.Template.Domain.Enums;

namespace CosmosDb.ChangeFeed.Template.WebApi.Models;

public sealed record ProductRequestModel(string Name, ProductStatus Status);