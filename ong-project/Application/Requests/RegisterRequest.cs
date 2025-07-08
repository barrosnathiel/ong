using System.Text.Json.Serialization;

namespace ong_project.Application.Requests;

public class RegisterRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("cpf")]
    public string CPF { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }
}