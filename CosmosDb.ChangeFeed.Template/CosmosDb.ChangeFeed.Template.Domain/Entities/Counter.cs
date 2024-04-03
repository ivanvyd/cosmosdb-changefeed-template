namespace CosmosDb.ChangeFeed.Template.Domain.Entities;

public sealed record Counter : Document
{
    public int Count { get; set; }
}
