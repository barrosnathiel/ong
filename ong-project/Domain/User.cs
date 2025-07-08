using System.ComponentModel.DataAnnotations;
namespace ong_project.Domain;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Cpf { get; set; }
    public string? ProfileImage { get; set; }
    public string? PhoneNumber { get; set; }
    public bool HasAcceptedTerms { get; set; }
    public string UserType { get; set; }
    public string Token { get; set; }
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}