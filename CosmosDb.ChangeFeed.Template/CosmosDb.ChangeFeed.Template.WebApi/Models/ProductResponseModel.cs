namespace CosmosDb.ChangeFeed.Template.WebApi.Models;

public class ProductResponseModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
