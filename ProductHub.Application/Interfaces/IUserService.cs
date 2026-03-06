using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Users.Dtos;

namespace ProductHub.Application.Interfaces;

public interface IUserService
{
    Task<Paged<UserResponseDto>> GetPagedUsersAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
