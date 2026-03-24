using System.ComponentModel.DataAnnotations;

namespace ProductHub.Application.Features.Auth.Dtos;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }
}
