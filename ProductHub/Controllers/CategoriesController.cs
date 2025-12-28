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
    [SwaggerOperation(
     Summary = "Gets Paged Categories in the database")]
    public async Task<IActionResult> GetPagedCategories([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var collaborators = await _categoryService.GetPagedCategories(query, cancellationToken);
        return Ok(BaseResponse.Ok(collaborators));
    }


    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get a single category by id")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpPost]
    [SwaggerOperation(
       Summary = "Creates a new category item")]
    public async Task<IActionResult> AddInventoryItem([FromBody] CategoryDto categoryDto, CancellationToken cancellationToken)
    {
        var result = await _categoryService.AddAsync(categoryDto, cancellationToken);
        return CreatedAtRoute(new { id = result.Id }, BaseResponse.Created(result));
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Updates an  existing Category")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CategoryDto request, CancellationToken cancellationToken = default)
    {
        await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(BaseResponse.Updated(request));
    }



    [HttpDelete("{id}")]
    [SwaggerOperation(
         Summary = "Deletes an inventory item and logs a comment explaining the reason for deletion",
         Description = "Deletes an inventory item resource identified by its ID and associates a provided comment as the reason for the deletion. The comment is logged for audit purposes."
         )]

    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return Ok(BaseResponse.Deleted(id));
    }
}
