using System.Text.Json.Serialization;

namespace ong_project.Application.Requests;

public class AuthRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}