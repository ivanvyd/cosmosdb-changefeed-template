using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosDb.ChangeFeed.Template.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ProductStatus
{
    New,
    Open,
    Closed,
}