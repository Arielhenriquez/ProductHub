using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Common.BaseResponse;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Categories.Dtos;
using ProductHub.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("paged")]
    [Authorize(Roles = "Admin,User")]
    [SwaggerOperation(Summary = "Gets Paged Categories (requires login)")]
    public async Task<IActionResult> GetPagedCategories([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var collaborators = await _categoryService.GetPagedCategories(query, cancellationToken);
        return Ok(BaseResponse.Ok(collaborators));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    [SwaggerOperation(Summary = "Get a single category by id (requires login)")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Creates a new category item (Admin only)")]
    public async Task<IActionResult> AddInventoryItem([FromBody] CategoryDto categoryDto, CancellationToken cancellationToken)
    {
        var result = await _categoryService.AddAsync(categoryDto, cancellationToken);
        return CreatedAtRoute(new { id = result.Id }, BaseResponse.Created(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Updates an existing Category (Admin only)")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CategoryDto request, CancellationToken cancellationToken = default)
    {
        await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(BaseResponse.Updated(request));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Deletes a category (soft delete, Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return Ok(BaseResponse.Deleted(id));
    }
}
