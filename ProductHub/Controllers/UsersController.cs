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

    [HttpPut("{id}/role")]
    [SwaggerOperation(Summary = "Update a user's role (Admin only)")]
    public async Task<IActionResult> UpdateRole([FromRoute] Guid id, [FromBody] UpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        await _userService.UpdateRoleAsync(id, dto, cancellationToken);
        return Ok(BaseResponse.Updated(dto));
    }

    [HttpPut("{id}/status")]
    [SwaggerOperation(Summary = "Enable or disable a user account (Admin only)")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateUserStatusDto dto, CancellationToken cancellationToken)
    {
        await _userService.UpdateStatusAsync(id, dto, cancellationToken);
        return Ok(BaseResponse.Updated(dto));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Soft-delete a user (Admin only)")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return Ok(BaseResponse.Deleted(id));
    }
}
