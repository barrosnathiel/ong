namespace ong_project.Domain.User;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Address { get; set; }
    public string Cpf { get; set; }
    public string ProfileImage { get; set; }
    public bool HasAcceptedTerms { get; set; }
    public string PhoneNumber { get; set; }
}