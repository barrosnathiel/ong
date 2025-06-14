using Microsoft.EntityFrameworkCore;
using ong_project.Domain.Repositories;
using ong_project.Domain.User;

namespace ong_project.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OngDbContext _context;

    public UserRepository(OngDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string cpf)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Cpf == cpf);
    }

    public Task<User> CreateUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}