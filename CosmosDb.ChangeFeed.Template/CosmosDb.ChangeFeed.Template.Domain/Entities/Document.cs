using Newtonsoft.Json;

namespace CosmosDb.ChangeFeed.Template.Domain.Entities;

/// <summary>
/// Represents a base document entity.
/// </summary>
public record Document
{
    /// <summary>
    /// Gets or sets the required document ID.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    public string? Type { get; set; }
}

