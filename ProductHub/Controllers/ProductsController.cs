using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Common.BaseResponse;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Products.Dtos;
using ProductHub.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Gets Paged Products (requires login)")]
    public async Task<IActionResult> GetPagedProducts([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var products = await _productService.GetPagedProducts(query, cancellationToken);
        return Ok(BaseResponse.Ok(products));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get a single product by id with images (requires login)")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Creates a new product (Admin only)")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto, CancellationToken cancellationToken)
    {
        var result = await _productService.AddAsync(createProductDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, BaseResponse.Created(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Updates an existing Product (Admin only)")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateProductDto request, CancellationToken cancellationToken)
    {
        await _productService.UpdateAsync(id, request, cancellationToken);
        return Ok(BaseResponse.Updated(request));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Deletes a product (soft delete, Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return Ok(BaseResponse.Deleted(id));
    }
}
