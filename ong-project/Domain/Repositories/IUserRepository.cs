namespace ong_project.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string cpf);
    Task<User?> GetByTokenAsync(string token);
    Task CreateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
}