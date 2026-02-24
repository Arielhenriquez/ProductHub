namespace ProductHub.Domain.Models;

public class Users : BaseEntity
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    /// <summary>"Admin" or "User"</summary>
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}
