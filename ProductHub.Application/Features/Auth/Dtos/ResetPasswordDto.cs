using System.ComponentModel.DataAnnotations;

namespace ProductHub.Application.Features.Auth.Dtos;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Token is required.")]
    public required string Token { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    public required string ConfirmPassword { get; set; }
}
