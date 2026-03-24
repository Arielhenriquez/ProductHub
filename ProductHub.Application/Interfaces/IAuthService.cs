using ProductHub.Application.Features.Auth.Dtos;

namespace ProductHub.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
}

