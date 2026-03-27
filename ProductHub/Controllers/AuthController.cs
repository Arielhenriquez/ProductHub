using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Common.BaseResponse;
using ProductHub.Application.Features.Auth.Dtos;
using ProductHub.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register a new user. Returns user data (no token). Use /login to get a token.")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return StatusCode(201, BaseResponse.Created(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login and get JWT access token")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(BaseResponse.Ok(result));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Request a password reset email. Always returns a generic response for security.")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(dto, cancellationToken);
        return Ok(new { message = "Te enviamos un correo con el enlace para restablecer tu contraseña. Revisa tu bandeja de entrada." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Reset password using a token received via email.")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(dto, cancellationToken);
        return Ok(new { message = "Password has been reset successfully." });
    }
}
