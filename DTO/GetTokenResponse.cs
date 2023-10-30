using System.Text.Json.Serialization;

namespace SampleMvcApp.DTO;

public sealed record GetTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}