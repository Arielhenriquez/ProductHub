using Newtonsoft.Json;

namespace ProductHub.Application.Common.TokensDtos;

public class TokenApiResponse
{
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public double ExpireIn { get; set; }

    [JsonProperty("error_description")]
    public string? Error { get; set; }
}
