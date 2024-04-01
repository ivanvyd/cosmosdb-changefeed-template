using CosmosDb.ChangeFeed.Template.Domain.Entities;
using CosmosDb.ChangeFeed.Template.Domain.Enums;

namespace CosmosDb.ChangeFeed.Template.Persistence.Repositories;

public interface IRepository
{
    Task CreateAsync(Product product);
    Task DeleteAsync(string id);
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetAsync(string id);
    Task<List<string>> GetByStatusAsync(ProductStatus status);
    Task<int> GetCountByStatusAsync(ProductStatus status);
    Task UpdateAsync(Product product);
}
