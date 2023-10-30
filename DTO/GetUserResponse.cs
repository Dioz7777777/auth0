using System.Text.Json.Serialization;

namespace SampleMvcApp.DTO;

public sealed record GetUserResponse
{
    [JsonPropertyName("email")]
    public string EmailAddress { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("picture")]
    public string ProfileImage { get; set; }
}