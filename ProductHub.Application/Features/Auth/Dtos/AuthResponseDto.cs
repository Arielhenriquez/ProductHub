namespace ProductHub.Application.Features.Auth.Dtos;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }  // seconds
    public AuthUserDto User { get; set; } = null!;
}

public class AuthUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
