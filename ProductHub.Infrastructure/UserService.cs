using Microsoft.EntityFrameworkCore;
using ProductHub.Application.Common.Exceptions;
using ProductHub.Application.Common.Extensions;
using ProductHub.Application.Common.Pagination;
using ProductHub.Application.Features.Users.Dtos;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Infrastructure;

public class UserService : IUserService
{
    private readonly IBaseRepository<Users> _userRepository;

    public UserService(IBaseRepository<Users> userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<Paged<UserResponseDto>> GetPagedUsersAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim().ToLower() ?? string.Empty;

        return _userRepository.Query()
            .AsNoTracking()
            .Where(u => !u.IsDeleted &&
                        (string.IsNullOrEmpty(search) ||
                         u.Email.ToLower().Contains(search) ||
                         u.FirstName.ToLower().Contains(search) ||
                         (u.LastName != null && u.LastName.ToLower().Contains(search))))
            .OrderByDescending(u => u.CreatedDate)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            })
            .Paginate(query.PageSize, query.PageNumber, cancellationToken);
    }

    public async Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(Users), id);

        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedDate = user.CreatedDate
        };
    }

    public async Task UpdateRoleAsync(Guid id, UpdateUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Role != "Admin" && dto.Role != "User")
            throw new BadRequestException("Role must be 'Admin' or 'User'.");

        var user = await GetUserOrThrowAsync(id, cancellationToken);
        user.Role = dto.Role;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateUserStatusDto dto, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(id, cancellationToken);
        user.IsActive = dto.IsActive;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(id, cancellationToken);
        user.IsDeleted = true;
        user.DeletedDate = DateTimeOffset.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    private async Task<Users> GetUserOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(Users), id);

        return user;
    }
}
