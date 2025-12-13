using Microsoft.AspNetCore.Mvc;
using WebApi.Demo.Models;
using WebApi.Demo.Services;

namespace WebApi.Demo.Controllers;

/// <summary>
/// Product management API controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Id = 0, // Will be assigned by service
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        var createdProduct = await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        var updatedProduct = await _productService.UpdateProductAsync(product);
        return Ok(updatedProduct);
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}

