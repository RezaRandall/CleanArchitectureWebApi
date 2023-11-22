using Application.Products.Queries;
using Application.Products.Commands;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Caching;
using Newtonsoft.Json;

namespace WebApiCleanArchitecture.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICachingService _cache;

    public ProductsController(ICachingService cache, IMediator mediator)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpGet]
    [Authorize] // This endpoint requires authentication
    public async Task<ActionResult<List<Product>>> GetAllProducts()
    {
        var cacheKey = "all_products"; // Key for caching the products

        // Attempt to retrieve products from cache
        var cachedProducts = await _cache.GetAsync(cacheKey);
        if (cachedProducts != null)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(cachedProducts);
            return Ok(products);
        }

        // If not found in cache, retrieve products from the mediator
        var query = new GetAllProductQuery();
        var productsFromMediator = await _mediator.Send(query);

        // Store products in cache for future use
        await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(productsFromMediator));

        return Ok(productsFromMediator);
    }

    [HttpGet("{name}")]
    [Authorize] // This endpoint requires authentication
    public async Task<ActionResult<Product>> GetProductByName(string name)
    {
        var cacheKey = $"product_{name}"; // Key for caching the product by name
        // Attempt to retrieve product from cache
        var cachedProduct = await _cache.GetAsync(cacheKey);
        if (cachedProduct != null)
        {
            var product = JsonConvert.DeserializeObject<Product>(cachedProduct);
            return Ok(product);
        }
        // If not found in cache, retrieve product from the mediator
        var query = new GetProductByNameQuery { Name = name };
        var productFromMediator = await _mediator.Send(query);
        if (productFromMediator == null)
            return NotFound();
        // Store product in cache for future use
        await _cache.SetAsync(cacheKey, JsonConvert.SerializeObject(productFromMediator));
        return Ok(productFromMediator);
    }

    [HttpPost]
    [Authorize] // This endpoint requires authentication
    public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return Ok(productId);
    }

    [HttpPut]
    [Authorize] // This endpoint requires authentication
    public async Task<ActionResult<bool>> UpdateProduct([FromBody] Product product)
    {
        // Set the Id received from the URL to the product's Id
        //var id = product.Id;
        //product.Id = id;

        var command = new UpdateProductCommand { Product = product };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize] // This endpoint requires authentication
    public async Task<ActionResult<bool>> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { ProductId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }


}
