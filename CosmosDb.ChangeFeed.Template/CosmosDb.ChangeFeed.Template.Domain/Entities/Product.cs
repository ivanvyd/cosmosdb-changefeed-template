using CosmosDb.ChangeFeed.Template.Domain.Enums;

namespace CosmosDb.ChangeFeed.Template.Domain.Entities;

public sealed record Product : Document
{
    public string? Name { get; set; }

    public ProductStatus Status { get; set; }
}
