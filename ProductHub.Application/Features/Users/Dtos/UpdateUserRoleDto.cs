namespace ProductHub.Application.Features.Users.Dtos;

public class UpdateUserRoleDto
{
    /// <summary>"Admin" or "User"</summary>
    public required string Role { get; set; }
}
