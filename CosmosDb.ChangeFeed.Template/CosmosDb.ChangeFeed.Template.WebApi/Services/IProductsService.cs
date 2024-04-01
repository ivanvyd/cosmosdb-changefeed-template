using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.WebApi.Models;

namespace CosmosDb.ChangeFeed.Template.WebApi.Services;

public interface IProductsService
{
    Task CreateProductAsync(ProductRequestModel productRequestModel);
    Task<Dictionary<ProductStatus, int>> GetProductsCountsAsync();
}
