using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductHub.Application.Common;
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
    private readonly IEmailService _emailService;

    // Reset-password token is its own short-lived JWT.  We re-use the same
    // signing secret as the main JwtSettings but distinguish it with a custom
    // "purpose" claim so it cannot be misused as a login token.
    private const string ResetPasswordPurpose = "reset-password";
    private const int ResetTokenExpiryMinutes = 15;

    public AuthService(
        IBaseRepository<Users> userRepository,
        IOptions<JwtSettings> jwtSettings,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
        _emailService = emailService;
    }

    // ──────────────────────────────────────────
    // Register
    // ──────────────────────────────────────────

    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Validate email format
        ValidateEmailFormat(dto.Email);

        // Validate password policy
        var passwordErrors = PasswordValidator.Validate(dto.Password);
        if (passwordErrors.Count > 0)
            throw new BadRequestException(string.Join(" | ", passwordErrors));

        // Check email uniqueness
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

        return new RegisterResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedDate = user.CreatedDate
        };
    }

    // ──────────────────────────────────────────
    // Login
    // ──────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        ValidateEmailFormat(dto.Email);

        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BadRequestException("Invalid email or password.");

        if (!user.IsActive)
            throw new BadRequestException("Your account is disabled. Contact an administrator.");

        return BuildAuthResponse(user);
    }

    // ──────────────────────────────────────────
    // Forgot Password
    // ──────────────────────────────────────────

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        // Always return success to avoid exposing whether the email exists
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted, cancellationToken);

        if (user is null)
            return; // Silently ignore: response is generic

        var resetToken = GenerateResetToken(user.Id);
        var resetLink = $"http://localhost:4200/reset-password?token={resetToken}";

        var subject = "Reset your password";
        var body = $@"
            <p>Hello {user.FirstName},</p>
            <p>We received a request to reset your ProductHub password.</p>
            <p>Click the link below to reset your password. This link expires in {ResetTokenExpiryMinutes} minutes.</p>
            <p><a href=""{resetLink}"">Reset My Password</a></p>
            <p>If you did not request a password reset, you can safely ignore this email.</p>
            <br/>
            <p>— The ProductHub Team</p>";

        await _emailService.SendAsync(user.Email, subject, body, cancellationToken);
    }

    // ──────────────────────────────────────────
    // Reset Password
    // ──────────────────────────────────────────

    public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        // Validate passwords match
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match.");

        // Validate password policy
        var passwordErrors = PasswordValidator.Validate(dto.NewPassword);
        if (passwordErrors.Count > 0)
            throw new BadRequestException(string.Join(" | ", passwordErrors));

        // Validate and parse the reset token
        var userId = ValidateResetToken(dto.Token);

        // Find user
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

        if (user is null)
            throw new BadRequestException("Invalid or expired password reset token.");

        // Hash and persist new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedDate = DateTimeOffset.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    // ──────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────

    private static void ValidateEmailFormat(string email)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            throw new BadRequestException($"'{email}' is not a valid email address.");
        }
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret.Trim()));
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

    /// <summary>
    /// Generates a short-lived JWT used exclusively for password reset.
    /// It includes a "purpose" claim so it cannot be used as a login token.
    /// </summary>
    private string GenerateResetToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret.Trim()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("purpose", ResetPasswordPurpose),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(ResetTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a reset token and returns the userId it contains.
    /// Throws BadRequestException if the token is invalid, expired, or has the wrong purpose.
    /// </summary>
    private Guid ValidateResetToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret.Trim()));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out _);

            // Ensure this is a reset-password token, not a login token
            var purpose = principal.FindFirstValue("purpose");
            if (purpose != ResetPasswordPurpose)
                throw new BadRequestException("Invalid or expired password reset token.");

            // JwtSecurityTokenHandler maps `sub` → ClaimTypes.NameIdentifier during validation
            var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(sub, out var userId))
                throw new BadRequestException("Invalid or expired password reset token.");

            return userId;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new BadRequestException("The password reset link has expired. Please request a new one.");
        }
        catch (SecurityTokenException)
        {
            throw new BadRequestException("Invalid or expired password reset token.");
        }
    }
}
