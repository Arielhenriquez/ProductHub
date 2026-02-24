using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Users.Dtos;

namespace ProductHub.Application.Interfaces;

public interface IUserService
{
    Task<Paged<UserResponseDto>> GetPagedUsersAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Guid id, UpdateUserRoleDto dto, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid id, UpdateUserStatusDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
