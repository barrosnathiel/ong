using System.Text.Json.Serialization;

namespace ong_project.Appllication.Requests;

public class UserLoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}