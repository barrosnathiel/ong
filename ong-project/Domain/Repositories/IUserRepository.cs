namespace ong_project.Domain.Repositories;

public interface IUserRepository
{
    Task<User.User?> GetByEmailAsync(string cpf);
    Task<User.User> CreateUserAsync(User.User user);
}