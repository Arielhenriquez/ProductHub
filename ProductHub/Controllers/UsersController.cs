using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Common.BaseResponse;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Users.Dtos;
using ProductHub.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductHub.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets paged users (Admin only)")]
    public async Task<IActionResult> GetPagedUsers([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var result = await _userService.GetPagedUsersAsync(query, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a single user by ID (Admin only)")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a user's name, email, role and/or status (Admin only)")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);
        return Ok(BaseResponse.Updated(result));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Soft-delete a user (Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return Ok(BaseResponse.Deleted(id));
    }
}
