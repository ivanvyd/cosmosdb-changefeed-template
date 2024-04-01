using CosmosDb.ChangeFeed.Template.Application.Factories;
using CosmosDb.ChangeFeed.Template.Application.Options;
using CosmosDb.ChangeFeed.Template.Domain.Entities;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.Persistence.Factories;
using CosmosDb.ChangeFeed.Template.WebApi.Models;
using Microsoft.Extensions.Options;

namespace CosmosDb.ChangeFeed.Template.WebApi.Services;

public sealed class ProductsService(IOptionsMonitor<AppSettingsOptions> appSettingsOptions,
    IRepositoriesFactory repositoriesFactory,
    ICountersFactory countersFactory) : IProductsService
{
    private readonly IOptionsMonitor<AppSettingsOptions> _appSettingsOptions = appSettingsOptions;

    private readonly IRepositoriesFactory _repositoriesFactory = repositoriesFactory;
    private readonly ICountersFactory _countersFactory = countersFactory;

    public async Task<Dictionary<ProductStatus, int>> GetProductsCountsAsync()
    {
        var response = new Dictionary<ProductStatus, int>();

        foreach (var status in Enum.GetValues<ProductStatus>())
        {
            int count = await _countersFactory
                .CreateCounter(status)
                .GetCountAsync();

            response.Add(status, count);
        }

        return response;
    }

    public async Task CreateProductAsync(ProductRequestModel productRequestModel)
    {
        ArgumentNullException.ThrowIfNull(productRequestModel);

        await _repositoriesFactory
            .CreateRepository(_appSettingsOptions.CurrentValue.DatabaseId, _appSettingsOptions.CurrentValue.ContainerId)
            .CreateAsync(new Product
            {
                Id = Guid.NewGuid().ToString(),
                Status = productRequestModel.Status,
                Name = productRequestModel.Name,
            });
    }
}
