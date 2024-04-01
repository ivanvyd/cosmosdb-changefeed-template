namespace CosmosDb.ChangeFeed.Template.Application.Options;

public sealed record AppSettingsOptions
{
    public const string SectionName = "AppSettings";

    public required string DatabaseId { get; init; }
    public required string ContainerId { get; init; }
}
