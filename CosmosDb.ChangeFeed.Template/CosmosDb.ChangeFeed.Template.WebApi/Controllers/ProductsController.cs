using CosmosDb.ChangeFeed.Template.Domain.Enums;
using CosmosDb.ChangeFeed.Template.WebApi.Models;
using CosmosDb.ChangeFeed.Template.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDb.ChangeFeed.Template.WebApi.Controllers;

[ApiController]
[Route("catalog/products")]
public sealed class ProductsController(IProductsService productsService) : Controller
{
    private readonly IProductsService _productsService = productsService;

    /// <summary>
    /// Retrieves the counts of products based on their status.
    /// </summary>
    /// <returns>A dictionary containing the product status as the key and the count as the value.</returns>
    [HttpGet("counts")]
    [ProducesResponseType(typeof(Dictionary<ProductStatus, int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<ProductStatus, int>>> GetProductsCountsAsync()
        => Ok(await _productsService.GetProductsCountsAsync());

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequestModel productRequestModel)
    {
        await _productsService.CreateProductAsync(productRequestModel);

        return NoContent();
    }
}
