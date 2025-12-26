namespace ProductHub.Domain.Models;

public class Users : BaseEntity
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsAdmin { get; set; }

}
