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

        return MapToDto(user);
    }

    public async Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(id, cancellationToken);

        // Name
        user.FirstName = dto.FirstName.Trim();
        user.LastName = string.IsNullOrWhiteSpace(dto.LastName) ? null : dto.LastName.Trim();

        // Email — validate uniqueness only when the address actually changes
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailTaken = await _userRepository.Query()
                .AnyAsync(u => u.Email == dto.Email && !u.IsDeleted && u.Id != id, cancellationToken);

            if (emailTaken)
                throw new BadRequestException($"Email '{dto.Email}' is already in use.");

            user.Email = dto.Email;
        }

        // Role — optional; validated when provided
        if (dto.Role is not null)
        {
            if (dto.Role != "Admin" && dto.Role != "User")
                throw new BadRequestException("Role must be 'Admin' or 'User'.");

            user.Role = dto.Role;
        }

        // Status — optional; updated only when explicitly provided
        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        user.UpdatedDate = DateTimeOffset.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetUserOrThrowAsync(id, cancellationToken);
        user.IsDeleted = true;
        user.DeletedDate = DateTimeOffset.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    // ──────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────

    private async Task<Users> GetUserOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(Users), id);

        return user;
    }

    private static UserResponseDto MapToDto(Users user) => new()
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
