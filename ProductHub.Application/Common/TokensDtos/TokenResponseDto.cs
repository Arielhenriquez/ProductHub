namespace ProductHub.Application.Common.TokensDtos;

public class TokenResponseDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ExpireTime { get; set; }
}
