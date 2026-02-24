using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductHub.Application.Common.Exceptions;
using ProductHub.Application.Features.Auth.Dtos;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;
using ProductHub.Domain.Settings;

namespace ProductHub.Infrastructure;

public class AuthService : IAuthService
{
    private readonly IBaseRepository<Users> _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IBaseRepository<Users> userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

        if (existingUser != null)
            throw new BadRequestException($"A user with email '{dto.Email}' already exists.");

        var user = new Users
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User",
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BadRequestException("Invalid email or password.");

        if (!user.IsActive)
            throw new BadRequestException("Your account is disabled. Contact an administrator.");

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(Users user)
    {
        var expireMinutes = _jwtSettings.ExpireMinutes;
        var token = GenerateJwtToken(user, expireMinutes);

        return new AuthResponseDto
        {
            AccessToken = token,
            ExpiresIn = expireMinutes * 60,
            User = new AuthUserDto
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    private string GenerateJwtToken(Users user, int expireMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
