using System.ComponentModel.DataAnnotations;

namespace ProductHub.Application.Features.Users.Dtos;

public class UpdateUserDto
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public required string FirstName { get; set; }

    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public required string Email { get; set; }

    /// <summary>"Admin" or "User". If omitted, the current role is kept.</summary>
    public string? Role { get; set; }

    /// <summary>If omitted, the current status is kept.</summary>
    public bool? IsActive { get; set; }
}
